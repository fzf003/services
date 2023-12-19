using Aliyun.OSS;
using AutoMapper;
using SCZS.CRMMS.Dto.PC.V2.Clue;
using SCZS.CRMMS.Dto.PC.V2.ClueTemplate;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCZS.CRMMS.ServicesPC.V2.Clue
{
    public class ClueRegisterMapProfile : BaseRegisterMapProfile
    {
        public ClueRegisterMapProfile()
        {

            #region 集团线索
              CreateMap<AddClueInput, Models.SCZSMK.clue>()
                     .ForMember(dest => dest.name, opt => opt.MapFrom(src => src.Customer.name))
                     .ForMember(dest => dest.sex, opt => opt.MapFrom(src => src.Customer.sex))
                     .ForMember(dest => dest.age, opt => opt.MapFrom(src => src.Customer.age))
                     .ForMember(dest => dest.phone, opt => opt.MapFrom(src => Encoding.UTF8.GetBytes(src.Customer.phone)))
                     .ForMember(dest => dest.weChat, opt => opt.MapFrom(src => src.Customer.weChat))
                     .ForMember(dest => dest.like_style, opt => opt.MapFrom(src => src.Customer.like_style))
                    
            


 
                   .ForMember(dest => dest.h_type2, opt => opt.MapFrom(src => src.Housing.h_type2))
                   .ForMember(dest => dest.house_type, opt => opt.MapFrom(src => src.Housing.house_type))
                   .ForMember(dest => dest.h_type, opt => opt.MapFrom(src => src.Housing.h_type))
                   .ForMember(dest => dest.handover_date, opt => opt.MapFrom(src => src.Housing.handover_date))
                   .ForMember(dest => dest.addresslist, opt => opt.MapFrom(src => src.Housing.addresslist))
                   .ForMember(dest => dest.from, opt => opt.MapFrom(src => src.Housing.from))
                   .ForMember(dest => dest.building_no, opt => opt.MapFrom(src => src.Housing.building_no))
                   .ForMember(dest => dest.area, opt => opt.MapFrom(src => src.Housing.area))
                   .ForMember(dest => dest.area_interval, opt => opt.MapFrom(src => src.Housing.area_interval))
                   .ForMember(dest => dest.note, opt => opt.MapFrom(src => src.Housing.note))
                   .ForMember(dest => dest.imageList, opt => opt.MapFrom(src => src.Housing.imageList))
                   .ForMember(dest => dest.intermediary, opt => opt.MapFrom(src => src.Housing.Intermediary))
                   .ForMember(dest => dest.contact_address, opt => opt.MapFrom(src => src.Housing.contact_address))
                   .ForMember(dest => dest.transaction_price, opt => opt.MapFrom(src => src.Housing.transaction_price))
                   .ForMember(dest => dest.payment_price, opt => opt.MapFrom(src => src.Housing.payment_price))
                   .ForMember(dest => dest.discount, opt => opt.MapFrom(src => src.Housing.Discount))
                   .ForMember(dest => dest.payment_ratio, opt => opt.MapFrom(src => src.Housing.payment_ratio))
                   .ForMember(dest => dest.discount_desc, opt => opt.MapFrom(src => src.Housing.discount_desc))
         
                   .ForMember(dest => dest.way, opt => opt.MapFrom(src => src.Source.way))
                   .ForMember(dest => dest.status, opt => opt.MapFrom(src => src.Source.status))
                   .ForMember(dest => dest.record_at, opt => opt.MapFrom(src => src.Source.record_at))
                   .ForMember(dest => dest.clue_level, opt => opt.MapFrom(src => src.Source.clue_level))
                   .ForMember(dest => dest.acquisition_type, opt => opt.MapFrom(src => src.Source.acquisition_type))
                   .ForMember(dest => dest.business_type, opt => opt.MapFrom(src => src.Source.business_type))
                   .ForMember(dest => dest.content_owner_cnum, opt => opt.MapFrom(src => src.Source.content_owner_cnum))
                   .ForMember(dest => dest.channel_category, opt => opt.MapFrom(src => src.Source.channel_category))
                   .ForMember(dest => dest.spread_account, opt => opt.MapFrom(src => src.Source.spread_account))
                   .ForMember(dest => dest.spread_account_id, opt => opt.MapFrom(src => src.Source.spread_account_id))
                   .ForMember(dest => dest.from_source, opt => opt.MapFrom(src => src.Source.from_source))
                   .ForMember(dest => dest.from_source_type, opt => opt.MapFrom(src => src.Source.from_source_type))
                   .ForMember(dest => dest.spread_channel, opt => opt.MapFrom(src => src.Source.spread_channel))
                   .ForMember(dest => dest.spread_sub_channel, opt => opt.MapFrom(src => src.Source.spread_sub_channel))
                   .ForMember(dest => dest.spread_category, opt => opt.MapFrom(src => src.Source.spread_category))
                   .ForMember(dest => dest.spread_title, opt => opt.MapFrom(src => src.Source.spread_title))
                   .ForMember(dest => dest.spread_ad_form, opt => opt.MapFrom(src => src.Source.spread_ad_form))
                   .ForMember(dest => dest.spread_site, opt => opt.MapFrom(src => src.Source.spread_site))
                   .ForMember(dest => dest.spread_ad, opt => opt.MapFrom(src => src.Source.spread_ad))
                   .ForMember(dest => dest.plan_name, opt => opt.MapFrom(src => src.Source.plan_name))
                   .ForMember(dest => dest.plan_id, opt => opt.MapFrom(src => src.Source.plan_id))
                   .ForMember(dest => dest.creative_id, opt => opt.MapFrom(src => src.Source.creative_id))
                   .ForMember(dest => dest.creative_name, opt => opt.MapFrom(src => src.Source.creative_name))
                   .ForMember(dest => dest.unit_ad_name, opt => opt.MapFrom(src => src.Source.unit_ad_name))
                   .ForMember(dest => dest.unit_ad_id, opt => opt.MapFrom(src => src.Source.unit_ad_id))
                   .ForMember(dest => dest.apply_page, opt => opt.MapFrom(src => src.Source.apply_page))
                   .ForMember(dest => dest.visit_ip, opt => opt.MapFrom(src => src.Source.visit_ip))
                   .ForMember(dest => dest.store_id, opt => opt.MapFrom(src => src.Source.store_id))
                   .ForMember(dest => dest.plat_type, opt => opt.MapFrom(src => src.Source.plat_type))
                 
                .ForMember(dest => dest.create_by_user, opt => opt.MapFrom(src => src.Team.create_user))
                .ForMember(dest => dest.create_at, opt => opt.MapFrom(src => src.Team.create_at))
                .ForMember(dest => dest.desiner_center_code, opt => opt.MapFrom(src => src.Team.desiner_center_code))
                .ForMember(dest => dest.desiner_center_name, opt => opt.MapFrom(src => src.Team.desiner_center_came))
                .ForMember(dest => dest.electric_helper_cnum, opt => opt.MapFrom(src => src.Team.electric_helper_cnum))
                .ForMember(dest => dest.electric_staff_cnum, opt => opt.MapFrom(src => src.Team.electric_staff_cnum))
                .ForMember(dest => dest.electric_manager_cnum, opt => opt.MapFrom(src => src.Team.electric_manager_cnum))
                //.ForMember(dest => dest.customer_manager_name, opt => opt.MapFrom(src => src.Team.customer_manager_name))
                .ForMember(dest => dest.c_owner, opt => opt.MapFrom(src => src.Team.c_owner))
                .ReverseMap();
            #endregion


            #region 客户表
            CreateMap<AddClueInput, Models.SCZSMK.consumer>()
              .ForMember(dest => dest.name, opt => opt.MapFrom(src => src.Customer.name))
              .ForMember(dest => dest.sex, opt => opt.MapFrom(src => src.Customer.sex))
              .ForMember(dest => dest.age, opt => opt.MapFrom(src => src.Customer.age))
              .ForMember(dest => dest.phone, opt => opt.MapFrom(src => src.Customer.phone))
              .ForMember(dest => dest.weChat, opt => opt.MapFrom(src => src.Customer.weChat))
              .ForMember(dest => dest.email, opt => opt.MapFrom(src => src.Customer.email))
              .ForMember(dest => dest.like_style, opt => opt.MapFrom(src => src.Customer.like_style))
             
                   .ForMember(dest => dest.h_type2, opt => opt.MapFrom(src => src.Housing.h_type2))
                   .ForMember(dest => dest.house_type, opt => opt.MapFrom(src => src.Housing.house_type))
                   .ForMember(dest => dest.h_type, opt => opt.MapFrom(src => src.Housing.h_type))
                   .ForMember(dest => dest.handover_date, opt => opt.MapFrom(src => src.Housing.handover_date))
                   .ForMember(dest => dest.address, opt => opt.MapFrom(src => src.Housing.addresslist))
                   .ForMember(dest => dest.floor_name, opt => opt.MapFrom(src => src.Housing.from))
                   .ForMember(dest => dest.building_no, opt => opt.MapFrom(src => src.Housing.building_no))
                   .ForMember(dest => dest.area, opt => opt.MapFrom(src => src.Housing.area))
                   .ForMember(dest => dest.area_interval, opt => opt.MapFrom(src => src.Housing.area_interval))
                   .ForMember(dest => dest.note, opt => opt.MapFrom(src => src.Housing.note))
                   .ForMember(dest => dest.imageList, opt => opt.MapFrom(src => src.Housing.imageList))
                   .ForMember(dest => dest.intermediary, opt => opt.MapFrom(src => src.Housing.Intermediary))
                   .ForMember(dest => dest.contact_address, opt => opt.MapFrom(src => src.Housing.contact_address))
                   .ForMember(dest => dest.transaction_price, opt => opt.MapFrom(src => src.Housing.transaction_price))
                   .ForMember(dest => dest.payment_price, opt => opt.MapFrom(src => src.Housing.payment_price))
                   .ForMember(dest => dest.discount, opt => opt.MapFrom(src => src.Housing.Discount))
                   .ForMember(dest => dest.payment_ratio, opt => opt.MapFrom(src => src.Housing.payment_ratio))
                   .ForMember(dest => dest.discount_desc, opt => opt.MapFrom(src => src.Housing.discount_desc))
                
                .ForMember(dest => dest.create_by_user, opt => opt.MapFrom(src => src.Team.create_user))
                .ForMember(dest => dest.create_at, opt => opt.MapFrom(src => src.Team.create_at))
                .ReverseMap();



            #endregion
 
            #region 派单表
            CreateMap<AddClueInput, Models.SCZSMK.sending_clues>()
                  .ForMember(dest => dest.consumer_name, opt => opt.MapFrom(src => src.Customer.name))
                  .ForMember(dest => dest.sex, opt => opt.MapFrom(src => src.Customer.sex))
                  .ForMember(dest => dest.consumer_phone, opt => opt.MapFrom(src => src.Customer.phone))
                  .ForMember(dest => dest.age, opt => opt.MapFrom(src => src.Customer.age))
                  .ForMember(dest => dest.weChat, opt => opt.MapFrom(src => src.Customer.weChat))
                  .ForMember(dest => dest.email, opt => opt.MapFrom(src => src.Customer.email))
                  .ForMember(dest => dest.like_style, opt => opt.MapFrom(src => src.Customer.like_style))
             
                   .ForMember(dest => dest.h_type2, opt => opt.MapFrom(src => src.Housing.h_type2))
                   .ForMember(dest => dest.house_type, opt => opt.MapFrom(src => src.Housing.house_type))
                   .ForMember(dest => dest.h_type, opt => opt.MapFrom(src => src.Housing.h_type))
                   .ForMember(dest => dest.handover_date, opt => opt.MapFrom(src => src.Housing.handover_date))
                   .ForMember(dest => dest.address, opt => opt.MapFrom(src => src.Housing.addresslist))
                   .ForMember(dest => dest.floor_name, opt => opt.MapFrom(src => src.Housing.from))
                   .ForMember(dest => dest.building_no, opt => opt.MapFrom(src => src.Housing.building_no))
                   .ForMember(dest => dest.area, opt => opt.MapFrom(src => src.Housing.area))
                   .ForMember(dest => dest.area_interval, opt => opt.MapFrom(src => src.Housing.area_interval))
                   .ForMember(dest => dest.note, opt => opt.MapFrom(src => src.Housing.note))
                   .ForMember(dest => dest.imageList, opt => opt.MapFrom(src => src.Housing.imageList))
                   .ForMember(dest => dest.intermediary, opt => opt.MapFrom(src => src.Housing.Intermediary))
                   .ForMember(dest => dest.contact_address, opt => opt.MapFrom(src => src.Housing.contact_address))
                   .ForMember(dest => dest.transaction_price, opt => opt.MapFrom(src => src.Housing.transaction_price))
                   .ForMember(dest => dest.payment_price, opt => opt.MapFrom(src => src.Housing.payment_price))
                   .ForMember(dest => dest.discount, opt => opt.MapFrom(src => src.Housing.Discount))
                   .ForMember(dest => dest.payment_ratio, opt => opt.MapFrom(src => src.Housing.payment_ratio))
                   .ForMember(dest => dest.discount_desc, opt => opt.MapFrom(src => src.Housing.discount_desc))
           
                   .ForMember(dest => dest.clue_way, opt => opt.MapFrom(src => src.Source.way))
                   .ForMember(dest => dest.cus_from, opt => opt.MapFrom(src => src.Source.from_source))
                
                   .ForMember(dest => dest.sending_user_cnum, opt => opt.MapFrom(src => src.Team.create_user))
                   .ForMember(dest => dest.created_at, opt => opt.MapFrom(src => src.Team.create_at))
                   .ForMember(dest => dest.c_owner, opt => opt.MapFrom(src => src.Team.c_owner))
                   .ForMember(dest => dest.taking_user_cnum, opt => opt.MapFrom(src => src.Team.market_manager_cnum))
                   .ForMember(dest => dest.taking_user_name, opt => opt.MapFrom(src => src.Team.market_manager_name))
                   .ForMember(dest => dest.market_director_cnum, opt => opt.MapFrom(src => src.Team.market_helper_cnum))
                   .ForMember(dest => dest.market_director_name, opt => opt.MapFrom(src => src.Team.market_helper_name))
                   .ForMember(dest => dest.market_connection_cnum, opt => opt.MapFrom(src => src.Team.market_staff_cnum))
                   .ForMember(dest => dest.market_connection_name, opt => opt.MapFrom(src => src.Team.market_staff_name))
                   .ReverseMap();











            #endregion

            #region 派单子表

            CreateMap<AddClueInput, Models.SCZSMK.sub_send_clue>()
                  .ForMember(dest => dest.consumer_name, opt => opt.MapFrom(src => src.Customer.name))
                  .ForMember(dest => dest.sex, opt => opt.MapFrom(src => src.Customer.sex))
                  .ForMember(dest => dest.consumer_phone, opt => opt.MapFrom(src => src.Customer.phone))
                  .ForMember(dest => dest.age, opt => opt.MapFrom(src => src.Customer.age))
                  .ForMember(dest => dest.weChat, opt => opt.MapFrom(src => src.Customer.weChat))
                  .ForMember(dest => dest.email, opt => opt.MapFrom(src => src.Customer.email))
                  .ForMember(dest => dest.like_style, opt => opt.MapFrom(src => src.Customer.like_style))
                 
                   .ForMember(dest => dest.h_type2, opt => opt.MapFrom(src => src.Housing.h_type2))
                   .ForMember(dest => dest.house_type, opt => opt.MapFrom(src => src.Housing.house_type))
                   .ForMember(dest => dest.h_type, opt => opt.MapFrom(src => src.Housing.h_type))
                   .ForMember(dest => dest.handover_date, opt => opt.MapFrom(src => src.Housing.handover_date))
                   .ForMember(dest => dest.address, opt => opt.MapFrom(src => src.Housing.addresslist))
                   .ForMember(dest => dest.floor_name, opt => opt.MapFrom(src => src.Housing.from))
                   .ForMember(dest => dest.building_no, opt => opt.MapFrom(src => src.Housing.building_no))
                   .ForMember(dest => dest.area, opt => opt.MapFrom(src => src.Housing.area))
                   .ForMember(dest => dest.area_interval, opt => opt.MapFrom(src => src.Housing.area_interval))
                   .ForMember(dest => dest.note, opt => opt.MapFrom(src => src.Housing.note))
                   .ForMember(dest => dest.imageList, opt => opt.MapFrom(src => src.Housing.imageList))
                   .ForMember(dest => dest.intermediary, opt => opt.MapFrom(src => src.Housing.Intermediary))
                   .ForMember(dest => dest.contact_address, opt => opt.MapFrom(src => src.Housing.contact_address))
                   .ForMember(dest => dest.transaction_price, opt => opt.MapFrom(src => src.Housing.transaction_price))
                   .ForMember(dest => dest.payment_price, opt => opt.MapFrom(src => src.Housing.payment_price))
                   .ForMember(dest => dest.discount, opt => opt.MapFrom(src => src.Housing.Discount))
                   .ForMember(dest => dest.payment_ratio, opt => opt.MapFrom(src => src.Housing.payment_ratio))
                   .ForMember(dest => dest.discount_desc, opt => opt.MapFrom(src => src.Housing.discount_desc))
             
                  .ForMember(dest => dest.market_manager_cnum, opt => opt.MapFrom(src => src.Team.create_user))
                  .ForMember(dest => dest.create_by, opt => opt.MapFrom(src => src.Team.create_user))
                  .ForMember(dest => dest.create_at, opt => opt.MapFrom(src => src.Team.create_at))
                  .ForMember(dest => dest.market_manager_cnum, opt => opt.MapFrom(src => src.Team.market_manager_cnum))
                  .ForMember(dest => dest.market_manager_name, opt => opt.MapFrom(src => src.Team.market_manager_name))
                  .ForMember(dest => dest.market_helper_cnum, opt => opt.MapFrom(src => src.Team.market_helper_cnum))
                  .ForMember(dest => dest.market_helper_name, opt => opt.MapFrom(src => src.Team.market_helper_name))
                  .ForMember(dest => dest.market_staff_cnum, opt => opt.MapFrom(src => src.Team.market_staff_cnum))
                  .ForMember(dest => dest.market_staff_name, opt => opt.MapFrom(src => src.Team.market_staff_name))
                  .ReverseMap();

            #endregion




 
        }
    }
}
