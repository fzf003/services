using AutoMapper;
using Castle.DynamicProxy.Generators.Emitters;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using SCZS.CRMMS.Dto;
using SCZS.CRMMS.Dto.PC.AutoDispatch.AutoOrderAcceptanceRules;
using SCZS.CRMMS.Dto.PC.V2.Clue;
using SCZS.CRMMS.Dto.User;
using SCZS.CRMMS.Dto.V2.Home;
using SCZS.CRMMS.IServices.V2.Customer;
using SCZS.CRMMS.IServicesPC.Common;
using SCZS.CRMMS.IServicesPC.Promotion;
using SCZS.CRMMS.IServicesPC.V2.Clue;
using SCZS.CRMMS.Models.SCZSMK;
using SCZS.CRMMS.Models.SCZSMK.DbContexts;
using SCZS.Public.Core.Common.SCZSOAuth;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace SCZS.CRMMS.ServicesPC.V2.Clue
{
    /// <summary>
    /// 线索服务
    /// </summary>

    public class ClueService : IClueService
    {
        private readonly SCZSMKContext _sczsContext;
        private readonly IUtilsService _utilsService;
        private IMapper _mapper;
        private readonly ICustomerService _customerService;
        private readonly IPromotionService _promotionService;

        public ClueService(SCZSMKContext sczsContext, IUtilsService utilsService, IMapper mapper, ICustomerService customerService, IPromotionService promotionService)
        {
            _sczsContext = sczsContext;
            _utilsService = utilsService;
            _mapper = mapper;
            _customerService = customerService;
            _promotionService = promotionService;
        }

        /// <summary>
        /// 新增线索
        /// </summary>
        /// <param name="addClueInput"></param>
        /// <returns></returns>
        public async Task<AppBaseOutput> add_clue(AddClueInput addClueInput)
        {
            AppBaseOutput output = new AppBaseOutput();

            var requestfilter = new ReqeustFilterContext();

            using (IDbContextTransaction transaction = _sczsContext.Database.BeginTransaction())
            {
                try
                {
                    await ValidateRequest(addClueInput, requestfilter, _utilsService);

                    /*_promotionService.muti_send_to_market(new Dto.Promotion.muti_send_to_market_Input { 

                    });*/
                    /* await _customerService.market_assign_clue(new Dto.V2.Customer.Market.market_assign_clueInput
                     {

                     });*/
 

                    if (requestfilter.IsOtherRoole)
                    {
                        await AddClue(addClueInput, requestfilter);
                    }
                    else if (requestfilter.IsNetSalesRole)
                    {
                        if (requestfilter.RoleName == "网销经理")
                        {
                           var addcule= await AddClue(addClueInput, requestfilter);
                          
                            await AddSendingClues(addClueInput, requestfilter,addcule);
                        }
                        else
                        {
                            var addcule = await AddClue(addClueInput, requestfilter);
                            
                           var sendclue= await AddSendingClues(addClueInput, requestfilter,addcule);

                            await AddSubSendClue(addClueInput, sendclue);
                        }
                    }


                    await _sczsContext.SaveChangesAsync();

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    output.IsSuccess = false;
                    output.Message = ex.Message;
                    await transaction.RollbackAsync();
                }


            }
        

            return output;
        }

        #region 验证请求

        /// <summary;>
        /// 中心角色: 中心经理, 客户经理, 设计师
        /// </summary>
        static readonly List<string> CenterRoles = new List<string>() { "中心经理", "客户经理", "设计师" };
        /// <summary>
        /// 网销角色
        /// </summary>
        static readonly List<string> NetFilterRoles = new List<string>() { "网销经理", "网销主管", "网销专员" };
        /// <summary>
        /// 验证请求
        /// </summary>
        /// <param name="addClueInput"></param>
        /// <returns></returns>
        async Task ValidateRequest(AddClueInput addClueInput, ReqeustFilterContext filterContext, IUtilsService utilsService)
        {
            var res = await utilsService.get_user_bpm_one();
            filterContext.BpmAccount = res.bpm;
            filterContext.Cnum = res.cnum;
            filterContext.RoleName = res.m_name;
            filterContext.CompanyName = LoginUserInfo.CompanyName;
            filterContext.CompanyCode = LoginUserInfo.CompanyCode;
            filterContext.DeptCode = LoginUserInfo.DeptCode;
            filterContext.DeptName = LoginUserInfo.DeptName;
            filterContext.Name = res.name;
            
 
            if (NetFilterRoles.Contains(filterContext.RoleName))
            {
                filterContext.IsNetSalesRole = true;

                if (string.IsNullOrWhiteSpace(addClueInput.Team.market_manager_cnum) || string.IsNullOrWhiteSpace(addClueInput.Team.market_manager_name))
                {
                    throw new Exception("客网经理为必填字段!");
                }


                if (filterContext.RoleName == "网销主管" || filterContext.RoleName == "网销专员")
                {
                    if (string.IsNullOrWhiteSpace(addClueInput.Team.market_helper_cnum) || string.IsNullOrWhiteSpace(addClueInput.Team.market_helper_name))
                    {
                        throw new Exception("客网主管为必填字段!");
                    }
                }

                if (filterContext.RoleName == "网销专员")
                {
                    if (string.IsNullOrWhiteSpace(addClueInput.Team.market_staff_name) || string.IsNullOrWhiteSpace(addClueInput.Team.market_staff_cnum))
                    {
                        throw new Exception("客网专员为必填字段!");
                    }
                }

            }
            else
            {
                filterContext.IsOtherRoole = true;
            }

        }

        #endregion


        #region 添加集团线索
        /// <summary>
        /// 添加集团线索
        /// </summary>
        /// <returns></returns>
        async Task<(clue,consumer)> AddClue(AddClueInput addClueInput, ReqeustFilterContext filterContext)
        {

            var addclue = _mapper.Map<clue>(addClueInput);

            var consumer =   _sczsContext.consumer.FirstOrDefault(p => p.phone == addClueInput.Customer.phone);
            if (consumer is null)
            {
                consumer = await AddConsumer(addClueInput);
            }
            else
            {
                string strpre = "CR" + DateTime.Now.ToString("yyMMdd");
                int mx = int.Parse(consumer.cnum.Substring(consumer.cnum.Length - 4, 4));
                string t = "0000"+ (mx + 1);
                consumer.cnum = strpre + t.Substring(t.Length - 4, 4);
            }

            Func<string> makecnum = () => {
                string date = "CL" + DateTime.Now.ToString("yyMMdd");
                var cnum = date + "0002";
                return cnum;
            };


            addclue.cnum = makecnum();
            addclue.consumer = consumer.cnum;
            addclue.valid = "true";
            //addclue.status = "";
            //addclue.clue_from_stack = "";
            addclue.progress = "market_confirm";
            addclue.company = filterContext.CompanyName;
            addclue.company_id = filterContext.CompanyCode;
            addclue.electric_mode = "unassigned";
            await _sczsContext.clue.AddAsync(addclue);
            await _sczsContext.SaveChangesAsync();
            return (addclue, consumer);

        }

        
        #endregion

        #region 添加客户信息表
        /// <summary>
        /// 添加客户信息表
        /// </summary>
        /// <param name="addClueInput"></param>
        /// <returns></returns>
        async Task<consumer> AddConsumer(AddClueInput addClueInput)
        {
            string strpre = "CR" + DateTime.Now.ToString("yyMMdd");
            var cnum = strpre + "0002";
            var addconsumer = _mapper.Map<consumer>(addClueInput);
            addconsumer.cnum= cnum;
            addconsumer.address = "{}";
            await _sczsContext.consumer.AddAsync(addconsumer);
            await _sczsContext.SaveChangesAsync();
            return addconsumer;
        }

        #endregion

        #region 添加派单表
        /// <summary>
        /// 添加客户信息表
        /// </summary>
        /// <param name="addClueInput"></param>
        /// <returns></returns>
        async Task<sending_clues> AddSendingClues(AddClueInput addClueInput, ReqeustFilterContext filterContext, (clue clue, consumer consumer) addclue)
        {
            var addsendingclues = _mapper.Map<sending_clues>(addClueInput);
            addsendingclues.prefix = "SC";
            addsendingclues.clue_cnum = addclue.clue.cnum;
            addsendingclues.consumer_cnum = addclue.consumer.cnum;
            //addsendingclues.clue_status = "";
            addsendingclues.taking_user_company = filterContext.CompanyName;
            addsendingclues.clue_level = addclue.clue.clue_level;
            addsendingclues.dept_code = filterContext.DeptCode.ToString();
            addsendingclues.dept_name = filterContext.DeptName;
            addsendingclues.clue_from_stack ="[]";
            addsendingclues.company = filterContext.CompanyName;
             addsendingclues.company_id=filterContext.CompanyCode;
            addsendingclues.sending_user_name = filterContext.Name;//派单人
            addsendingclues.sending_user_cnum = filterContext.BpmAccount;//派单人

            await _sczsContext.sending_clues.AddAsync(addsendingclues);
            await _sczsContext.SaveChangesAsync();

            addsendingclues.assign_cnum = _utilsService._getSCnum(addsendingclues.id.ToString());

            return addsendingclues;
        }
 
        #endregion

        #region 添加派单子表

        /// <summary>
        /// 添加子派单表  
        /// </summary>
        /// <param name="addClueInput"></param>
        /// <returns></returns>
        async Task AddSubSendClue(AddClueInput addClueInput, sending_clues sending)
        {
            var addsubsendclue = _mapper.Map<sub_send_clue>(addClueInput);

             addsubsendclue.assign_cnum = sending.assign_cnum;
             addsubsendclue.progress= sending.progress;
             addsubsendclue.consumer_cnum= sending.consumer_cnum;
 

            await _sczsContext.sub_send_clue.AddAsync(addsubsendclue);
            await _sczsContext.SaveChangesAsync();

        }

        #endregion

      
      

    }
}
