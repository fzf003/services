using Microsoft.EntityFrameworkCore;
using Nacos.V2.Utils;
using NLog.LayoutRenderers;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using Org.BouncyCastle.Math;
using SCZS.CQS.Application.Contracts.Dtos.House;
using SCZS.CQS.Application.Contracts.Dtos.QuotedPrice.ProductSelection;
using SCZS.CQS.Application.Contracts.Service.QuotedPrice;
using SCZS.CQS.Infrastructures.Extensions;
using SCZS.CQS.Model.Extensions;
using SCZS.CQS.Model.Extensions.Entities;
using SCZS.CQS.Model.SCZSCQS;
using SCZS.RestClient.Rto.ScmnClient;
using SqlSugar;
using StackExchange.Profiling.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace SCZS.CQS.Application.Service.QuotedPrice
{
    /// <summary>
    /// 报价选品
    /// </summary>
    public partial class QuotedPriceAppService : IQuotedPriceAppService
    {
        #region 获取品类列表
        /// <summary>
        /// 获取品类列表
        /// </summary>
        /// <returns></returns>
        public async Task<Output<List<GetProductCategoryListOuput>>> GetProductCategoryList(GetProductCategoryListInput listInput)
        {
            Output<List<GetProductCategoryListOuput>> output = new Output<List<GetProductCategoryListOuput>>();
            output.IsSuccess = true;
            output.Data = new List<GetProductCategoryListOuput>();

            try
            {
                if (listInput.PmPriceId <= 0)
                {
                    return output;
                }

                var PmPrice = await this._quotedPriceManager.GetQuotedPriceM(listInput.PmPriceId);
                if (PmPrice is null)
                {
                    return output;
                }

                var RequiredItemquery = await (from config in this.DbContext.CQS_RequiredItem_Config.AsNoTracking().WithHint(SqlServerTableHintFlags.NOLOCK).Where(p => p.CompanyCode == PmPrice.CompanyCode && p.IsUse == 1)
                                               from item in this.DbContext.CQS_RequiredItem_Config_T.AsNoTracking().WithHint(SqlServerTableHintFlags.NOLOCK)
                                               where config.Code == item.Code
                                               select item).Select(p => p.ProductCategoryCode).ToListAsync();


                var categorys = await this.DbContext.V_PM_SCMCategory.AsNoTracking().WithHint(SqlServerTableHintFlags.NOLOCK).OrderByDescending(p => p.CreateTime).ToListAsync();

                if (categorys.Any())
                {
                    var categorysorerbyfilter = categorys.Select((p, index) => new GetProductCategoryListOuput
                    {
                        CategoryCode = p.CategoryCode,
                        CategoryName = p.CategoryName,
                        SortNum = index + 1,
                        IsRequired = RequiredItemquery.Contains(p.CategoryCode) ? 1 : 0,
                        CategoryLogo = p.CategoryLogo
                    });

                    var categorysorerbylist = categorysorerbyfilter.Any(p => p.IsRequired == 1) switch
                    {
                        true => categorysorerbyfilter.OrderByDescending(p => p.IsRequired).ThenByDescending(p => p.SortNum),
                        false => categorysorerbyfilter.OrderByDescending(p => p.SortNum)
                    };

                    output.Data.AddRange(categorysorerbylist);
                }

            }
            catch (Exception ex)
            {
                output.IsSuccess = false;
                output.Message = "查询出错";
            }
            return output;
        }




        #endregion


        #region 产品搜索项
        /// <summary>
        /// 产品搜索项
        /// </summary>
        /// <param name="categoryInput"></param>
        /// <returns></returns>
        public async Task<Output<GetProductSearchCategoryOutput>> GetProductSearchCategorys(GetProductSearchCategoryInput categoryInput)
        {
            Output<GetProductSearchCategoryOutput> output = new Output<GetProductSearchCategoryOutput>();
            output.IsSuccess = true;
            GetProductSearchCategoryOutput searchCategoryOutput = new GetProductSearchCategoryOutput();

            try
            {
                Expression<Func<V_PM_ProductSearchCategory, bool>> filterexpress = ExpressionCreator.New<V_PM_ProductSearchCategory>();

                if (categoryInput.CompanyCode <= 0)
                {
                    return output;
                }

                filterexpress = filterexpress.And(p => p.CompanyCode == categoryInput.CompanyCode);
                if (!string.IsNullOrWhiteSpace(categoryInput.ProductTypeCode))
                {
                    filterexpress = filterexpress.And(p => p.ProductTypeCode == categoryInput.ProductTypeCode);
                }

                var SearchCategorys = await this.DbContext.V_PM_ProductSearchCategory.AsNoTracking().WithHint(SqlServerTableHintFlags.NOLOCK).Where(filterexpress).ToListAsync();

                if (SearchCategorys.Any())
                {
                    var groupedCategorys = SearchCategorys.GroupBy(p => p.SearchItemCode)
                           .Select(g => new
                           {
                               SearchItemCode = g.Key,
                               Categorys = g
                           });

                    foreach (var group in groupedCategorys)
                    {

                        if (group.SearchItemCode == 1)
                        {
                            searchCategoryOutput.Brands.AddRange(group.Categorys.Select(p => new SearchCategory
                            {
                                CompanyCode = p.CompanyCode,
                                SearchItemName = p.SearchItemName,
                                CompanyName = p.CompanyName,
                                ItemCode = p.ItemCode,
                                ItemName = p.ItemName,
                                SearchItemCode = p.SearchItemCode
                            }));
                        }

                        if (group.SearchItemCode == 2)
                        {
                            searchCategoryOutput.Spces.AddRange(group.Categorys.Select(p => new SearchCategory
                            {
                                CompanyCode = p.CompanyCode,
                                SearchItemName = p.SearchItemName,
                                CompanyName = p.CompanyName,
                                ItemCode = p.ItemCode,
                                ItemName = p.ItemName,
                                SearchItemCode = p.SearchItemCode
                            }));
                        }

                        if (group.SearchItemCode == 3)
                        {
                            searchCategoryOutput.Colors.AddRange(group.Categorys.Select(p => new SearchCategory
                            {
                                CompanyCode = p.CompanyCode,
                                SearchItemName = p.SearchItemName,
                                CompanyName = p.CompanyName,
                                ItemCode = p.ItemCode,
                                ItemName = p.ItemName,
                                SearchItemCode = p.SearchItemCode
                            }));
                        }

                        if (group.SearchItemCode == 4)
                        {
                            searchCategoryOutput.Series.AddRange(group.Categorys.Select(p => new SearchCategory
                            {
                                CompanyCode = p.CompanyCode,
                                SearchItemName = p.SearchItemName,
                                CompanyName = p.CompanyName,
                                ItemCode = p.ItemCode,
                                ItemName = p.ItemName,
                                SearchItemCode = p.SearchItemCode
                            }));
                        }

                        if (group.SearchItemCode == 5)
                        {
                            searchCategoryOutput.MaterialQualitys.AddRange(group.Categorys.Select(p => new SearchCategory
                            {
                                CompanyCode = p.CompanyCode,
                                SearchItemName = p.SearchItemName,
                                CompanyName = p.CompanyName,
                                ItemCode = p.ItemCode,
                                ItemName = p.ItemName,
                                SearchItemCode = p.SearchItemCode
                            }));
                        }

                    }

                    output.Data = searchCategoryOutput;
                }

            }
            catch (Exception ex)
            {
                output.IsSuccess = false;
                output.Message = "查询出错";
            }

            return output;
        }
        #endregion


        #region 产品选品信息列表
        /// <summary>
        /// 产品选品信息列表
        /// </summary>
        /// <param name="productInput"></param>
        /// <returns></returns>
        public async Task<Output<PageOutEntity<GetSearchProductInfoOutput>>> GetSearchProductInfo(GetSearchProductInfoInput productInput)
        {
            Output<PageOutEntity<GetSearchProductInfoOutput>> output = new Output<PageOutEntity<GetSearchProductInfoOutput>>();
            output.Data = new PageOutEntity<GetSearchProductInfoOutput>();
            output.Data.Total = 0;
            output.Data.Children = new List<GetSearchProductInfoOutput>();

            var IsEmptyProductType = string.IsNullOrWhiteSpace(productInput.ProductTypeCode);

            bool IsZGProductLine = false;


            try
            {
                var PmPrice = await this._quotedPriceManager.GetQuotedPriceM(productInput.PmPriceId);
                if (PmPrice is null)
                {
                    return output;
                }


                IsZGProductLine = PmPrice.QPlateCode switch
                {

                    1 => true,
                    _ => false
                };



                #region 产品条件查询

                //品类条件为空：IsEmptyProductType,整装产品线,优先展示主推产品 然后显示必报项 和非必报项
                //品类条件为空：IsEmptyProductType 施工包和个性化 优先展示主推产品 然后显示零售产品
                //空间、部位、品类 条件必须满足，展示整装产品 优先级 主推产品 必报项产品  非必报项（零售） 否则 零售。


                var (productfilterexpression, ishidproductfilter, ishidSpacefilter, IsInputfilter, IsReplacefilter, partIds) = await GetProductFilterExpression(productInput, PmPrice);

                PageOutEntity<V_PM_SelectedProductSearch> pagerproductquery = new PageOutEntity<V_PM_SelectedProductSearch>();


                if (IsZGProductLine)
                {
                    if (ishidSpacefilter && ishidproductfilter && !IsEmptyProductType)///品类不为空
                    {

                        var productfilter = this.DbContext.V_PM_SelectedProductSearch.AsNoTracking().WithHint(SqlServerTableHintFlags.NOLOCK).Where(productfilterexpression).Where(p => p.SaleWay == 3);


                        var ProductLine = this.DbContext.V_PM_SelectedProductSearch.AsNoTracking().WithHint(SqlServerTableHintFlags.NOLOCK).Where(p => p.ProductLineCode == PmPrice.ProductLineCode).Where(p => partIds.Contains(p.PID ?? -1));//.ToListAsync();
                         
                        var query = from item in productfilter
                                    from pitem in ProductLine
                                    let IsHidproduct = ProductLine.Where(p => p.ProductCode == item.ProductCode)
                                    where item.ProductCode == pitem.ProductCode
                                    select new GetSearchProductInfoOutput
                                    {
                                        BrandCode = item.BrandCode,
                                        BrandName = item.BrandName,
                                        Color = item.Color,
                                        ImgUrl = item.ImgUrl,
                                        IsMain = item.IsMain,
                                        Marque = item.Marque,
                                        MaterialCode = item.MaterialCode,
                                        MaterialQuality = item.MaterialQuality,
                                        ProductCategory = item.ProductCategory,
                                        ProductCategoryCode = item.ProductCategoryCode,
                                        ProductSubclass = item.ProductSubclass,
                                        ProductType = item.ProductType,
                                        Series = item.Series,
                                        SmallImgUrl = item.SmallImgUrl,
                                        SpecHigh = item.SpecHigh ?? 0,
                                        SpecLong = item.SpecLong ?? 0,
                                        SpecWide = item.SpecWide ?? 0,
                                        ProductCode = item.ProductCode,
                                        ProductName = item.ProductName,
                                        ProductSubclassCode = item.ProductSubclassCode,
                                        ProductTypeCode = item.ProductTypeCode,
                                        SaleWay = IsHidproduct.Any() ? IsHidproduct.Select(p => p.SaleWay).ToList() : new List<int>() { item.SaleWay },
                                        SalesUnitPrice = IsHidproduct.Any() ? 0 : pitem.SalesUnitPrice ?? 0m,
                                        SJUnitPrice = IsHidproduct.Any() ? (pitem.SaleWay == 2 ? pitem.SalesUnitPrice ?? 0m : 0m) : 0m,
                                        StandardUnitPrice = 0
                                         


                                    };






                        var productorderbyfilter = productInput.SaleOrder switch
                        {
                            1 => productfilter.OrderBy(p => p.SalesUnitPrice),
                            2 => productfilter.OrderByDescending(p => p.SalesUnitPrice),
                            0 => productfilter.OrderByDescending(p => p.IsMain).ThenByDescending(p => p.IsRequired).ThenBy(p => p.SaleWay),
                            _ => productfilter.OrderByDescending(p => p.IsMain).ThenByDescending(p => p.IsRequired).ThenBy(p => p.SaleWay)
                        };



                        // pagerproductquery = await productorderbyfilter.ToQueryPagedAsync(productInput.Page, productInput.Limit);

                    }
                    else if (IsReplacefilter)//制作报价替换
                    {
                        var replaceproductfilter = this.DbContext.V_PM_SelectedProductSearch.AsNoTracking().WithHint(SqlServerTableHintFlags.NOLOCK).Where(productfilterexpression);

                        var replaceproductorderbyfilter = productInput.SaleOrder switch
                        {
                            1 => replaceproductfilter.OrderBy(p => p.SalesUnitPrice),
                            2 => replaceproductfilter.OrderByDescending(p => p.SalesUnitPrice),
                            0 => replaceproductfilter.OrderByDescending(p => p.IsMain).ThenByDescending(p => p.IsRequired).ThenBy(p => p.SaleWay),
                            _ => replaceproductfilter.OrderByDescending(p => p.IsMain).ThenByDescending(p => p.IsRequired).ThenBy(p => p.SaleWay)
                        };

                        pagerproductquery = await replaceproductorderbyfilter.ToQueryPagedAsync(productInput.Page, productInput.Limit);
                    }
                    else //展示零售产品
                    {
                        var lsproductfilter = this.DbContext.V_PM_SelectedProductSearch.AsNoTracking().WithHint(SqlServerTableHintFlags.NOLOCK).Where(p => p.SaleWay == 3).Where(productfilterexpression);
                        var lsproductorderbyfilter = productInput.SaleOrder switch
                        {
                            1 => lsproductfilter.OrderBy(p => p.SalesUnitPrice),
                            2 => lsproductfilter.OrderByDescending(p => p.SalesUnitPrice),
                            0 => lsproductfilter.OrderByDescending(p => p.IsMain).ThenByDescending(p => p.IsRequired).ThenBy(p => p.SaleWay),
                            _ => lsproductfilter.OrderByDescending(p => p.IsMain).ThenByDescending(p => p.IsRequired).ThenBy(p => p.SaleWay)
                        };

                        pagerproductquery = await lsproductorderbyfilter.ToQueryPagedAsync(productInput.Page, productInput.Limit);
                    }


                }
                else //施工包和个性化
                {
                    if (IsReplacefilter)//制作报价替换
                    {
                        var replaceproductfilter = this.DbContext.V_PM_SelectedProductSearch.AsNoTracking().WithHint(SqlServerTableHintFlags.NOLOCK).Where(productfilterexpression);

                        var replaceproductorderbyfilter = productInput.SaleOrder switch
                        {
                            1 => replaceproductfilter.OrderBy(p => p.SalesUnitPrice),
                            2 => replaceproductfilter.OrderByDescending(p => p.SalesUnitPrice),
                            0 => replaceproductfilter.OrderByDescending(p => p.IsMain).ThenByDescending(p => p.IsRequired).ThenBy(p => p.SaleWay),
                            _ => replaceproductfilter.OrderByDescending(p => p.IsMain).ThenByDescending(p => p.IsRequired).ThenBy(p => p.SaleWay)
                        };

                        pagerproductquery = await replaceproductorderbyfilter.ToQueryPagedAsync(productInput.Page, productInput.Limit);
                    }
                    else
                    {
                        var lsproductfilter = this.DbContext.V_PM_SelectedProductSearch.AsNoTracking().WithHint(SqlServerTableHintFlags.NOLOCK).Where(p => p.SaleWay == 3).Where(productfilterexpression);
                        var lsproductorderbyfilter = productInput.SaleOrder switch
                        {
                            1 => lsproductfilter.OrderBy(p => p.SalesUnitPrice),
                            2 => lsproductfilter.OrderByDescending(p => p.SalesUnitPrice),
                            0 => lsproductfilter.OrderByDescending(p => p.IsMain).ThenByDescending(p => p.IsRequired).ThenBy(p => p.SaleWay),
                            _ => lsproductfilter.OrderByDescending(p => p.IsMain).ThenByDescending(p => p.IsRequired).ThenBy(p => p.SaleWay)
                        };

                        pagerproductquery = await this.DbContext.V_PM_SelectedProductSearch.AsNoTracking().WithHint(SqlServerTableHintFlags.NOLOCK).Where(p => p.SaleWay == 3).Where(productfilterexpression).ToQueryPagedAsync(productInput.Page, productInput.Limit);
                    }
                }

                #endregion

                output.Data.Total = pagerproductquery.Total;

                output.Data.Children = this.Mapper.Map<List<GetSearchProductInfoOutput>>(pagerproductquery.Children);

            }
            catch (Exception ex)
            {
                output.IsSuccess = false;
                output.Message = ex.Message;
            }

            return output;
        }

        /// <summary>
        /// 搜索条件
        /// </summary>
        /// <param name="partInfoInput"></param>
        /// <returns></returns>
        async Task<(Expression<Func<V_PM_SelectedProductSearch, bool>> expression, bool isProductfilter, bool isSpacefilter, bool IsInputfilter, bool IsReplacefilter, List<long> partids)> GetProductFilterExpression(GetSearchProductInfoInput getSearchProduct, CQS_QuotedPrice_M priceM)
        {
            bool isProductfilter = false;//产品快捷搜索

            bool isSpacefilter = false;//空间关联搜索

            bool IsInputfilter = false;//输入框搜索

            bool IsReplacefilter = false;//制作报价替换入口

            var IsZGProductLine = priceM.QPlateCode switch
            {

                1 => true,
                _ => false
            };

            List<long> partIds = new List<long>();

            Expression<Func<V_PM_SelectedProductSearch, bool>> filterexpress = ExpressionCreator.New<V_PM_SelectedProductSearch>();

            filterexpress = filterexpress.And(p => p.CompanyCode == priceM.CompanyCode);

            #region 包含指定项目的临时物料
            ///指定项目的临时物料
            var MaterialCodes = await this.DbContext.V_PM_MaterialAppointProject.AsNoTracking().WithHint(SqlServerTableHintFlags.NOLOCK)
                                      .Where(p => p.CusCode == priceM.CusCode)
                                      .Select(p => p.MaterialCode).ToListAsync();
            #endregion

            //  制作报价替换入口
            if (!string.IsNullOrWhiteSpace(getSearchProduct.Replace_ProductTypeCode) && !string.IsNullOrWhiteSpace(getSearchProduct.Replace_ProductSubclassCode))
            {
                #region 制作报价替换入口
                if (IsZGProductLine)
                {
                    var ids = await this.DbContext.V_PM_MatchProductLineSpacePart.AsNoTracking().WithHint(SqlServerTableHintFlags.NOLOCK)
                                        .Where(p => p.ProductLineCode == priceM.ProductLineCode && p.SpaceCode == getSearchProduct.Replace_SpaceCode && p.PartCode == getSearchProduct.Replace_PartCode)
                                        .Select(p => p.ID).ToListAsync();

                    if (ids.Any())
                    {
                        filterexpress = filterexpress.And(p => ids.Contains(p.PID ?? -1) || p.SaleWay == 3);
                    }
                    else
                    {
                        filterexpress = filterexpress.And(p => p.ProductTypeCode == getSearchProduct.Replace_ProductTypeCode)
                                                     .And(p => p.ProductSubclassCode == getSearchProduct.Replace_ProductSubclassCode);



                        ///标签搜索
                        filterexpress = getSearchProduct.TagCode switch
                        {
                            "标配" => filterexpress.And(p => p.SaleWay == 1),
                            "升级" => filterexpress.And(p => p.SaleWay == 2),
                            "主推" => filterexpress.And(p => p.IsMain == 1),
                            _ => filterexpress
                        };

                    }

                    if (MaterialCodes.Any())
                    {
                        filterexpress = filterexpress.And(p => MaterialCodes.Contains(p.MaterialCode));
                    }


                }
                else
                {
                    filterexpress = filterexpress.And(p => p.ProductTypeCode == getSearchProduct.Replace_ProductTypeCode)
                                                 .And(p => p.ProductSubclassCode == getSearchProduct.Replace_ProductSubclassCode);
                    ///标签搜索
                    filterexpress = getSearchProduct.TagCode switch
                    {
                        "标配" => filterexpress.And(p => p.SaleWay == 1),
                        "升级" => filterexpress.And(p => p.SaleWay == 2),
                        "主推" => filterexpress.And(p => p.IsMain == 1),
                        _ => filterexpress
                    };
                }

                if (!string.IsNullOrWhiteSpace(getSearchProduct.Replace_Color) || !string.IsNullOrWhiteSpace(getSearchProduct.MaterialQualitys) || !string.IsNullOrWhiteSpace(getSearchProduct.Replace_Spec))
                {

                    Expression<Func<V_PM_SelectedProductSearch, bool>> filterorexpress = ExpressionCreator.New<V_PM_SelectedProductSearch>();

                    if (!string.IsNullOrWhiteSpace(getSearchProduct.Replace_Color))
                    {
                        filterorexpress = filterorexpress.Or(p => p.Color == getSearchProduct.Replace_Color);
                    }

                    if (!string.IsNullOrWhiteSpace(getSearchProduct.MaterialQualitys))
                    {
                        filterorexpress = filterorexpress.Or(p => p.MaterialQuality == getSearchProduct.MaterialQualitys);
                    }

                    if (!string.IsNullOrWhiteSpace(getSearchProduct.Replace_Spec))
                    {
                        filterorexpress = filterorexpress.Or(p => p.Spec == getSearchProduct.Replace_Spec);
                    }

                    filterexpress = filterexpress.And(filterorexpress);
                }

                IsReplacefilter = true;
                #endregion
            }
            else
            {
                #region 空间部位搜索条件
                ///整装产品线可以按空间部位搜索,其他不必
                if (IsZGProductLine)
                {
                    filterexpress = filterexpress.And(p => p.ProductLineCode == priceM.ProductLineCode);

                    if (!string.IsNullOrWhiteSpace(getSearchProduct.PartCode) && !string.IsNullOrWhiteSpace(getSearchProduct.SpaceCode))
                    {
                        var ids = await this.DbContext.V_PM_MatchProductLineSpacePart.AsNoTracking().WithHint(SqlServerTableHintFlags.NOLOCK)
                                        .Where(p => p.ProductLineCode == priceM.ProductLineCode && p.SpaceCode == getSearchProduct.SpaceCode && p.PartCode == getSearchProduct.PartCode)
                                        .Select(p => p.ID).ToListAsync();
                        if (ids.Any())
                        {
                            partIds.AddRange(ids);

                            //filterexpress = filterexpress.And(p => partIds.Contains(p.PID ?? -1));


                        }

                        isSpacefilter = true;
                    }
                }

                #endregion


                #region 快捷搜索
                if (!string.IsNullOrWhiteSpace(getSearchProduct.Spces))//规格
                {
                    filterexpress = filterexpress.And(p => p.Spec == getSearchProduct.Spces);
                    isProductfilter = true;
                }

                if (!string.IsNullOrWhiteSpace(getSearchProduct.Brands))//品牌
                {
                    filterexpress = filterexpress.And(p => p.BrandCode == getSearchProduct.Brands);
                    isProductfilter = true;
                }

                if (!string.IsNullOrWhiteSpace(getSearchProduct.ProductTypeCode))//品类
                {
                    filterexpress = filterexpress.And(p => p.ProductTypeCode == getSearchProduct.ProductTypeCode);
                    isProductfilter = true;
                }

                if (!string.IsNullOrWhiteSpace(getSearchProduct.Colors))//颜色
                {
                    filterexpress = filterexpress.And(p => p.Color.Contains(getSearchProduct.Colors));
                    isProductfilter = true;
                }

                if (!string.IsNullOrWhiteSpace(getSearchProduct.Series))//系列
                {
                    filterexpress = filterexpress.And(p => p.Series.Contains(getSearchProduct.Series));
                    isProductfilter = true;
                }

                if (!string.IsNullOrWhiteSpace(getSearchProduct.MaterialQualitys))//材质
                {
                    filterexpress = filterexpress.And(p => p.MaterialQuality.Contains(getSearchProduct.MaterialQualitys));
                    isProductfilter = true;
                }

                #endregion


                #region 输入框搜索

                if (!string.IsNullOrWhiteSpace(getSearchProduct.AllKeyWords))
                {
                    filterexpress = filterexpress.And(p => p.ProductionPlace.Contains(getSearchProduct.AllKeyWords)
                    || p.Marque.Contains(getSearchProduct.AllKeyWords)
                    || p.ProductName.Contains(getSearchProduct.AllKeyWords)
                    || p.BrandName.Contains(getSearchProduct.AllKeyWords)
                    || p.Color.Contains(getSearchProduct.AllKeyWords)
                    || p.Series.Contains(getSearchProduct.AllKeyWords)
                    || p.MaterialQuality.Contains(getSearchProduct.AllKeyWords)
                    || p.ProductionPlace.Contains(getSearchProduct.AllKeyWords)
                    );

                    IsInputfilter = true;
                }


                #endregion

                #region 标签搜索
                filterexpress = getSearchProduct.TagCode switch
                {
                    "标配" => filterexpress.And(p => p.SaleWay == 1),
                    "升级" => filterexpress.And(p => p.SaleWay == 2),
                    "主推" => filterexpress.And(p => p.IsMain == 1),
                    _ => filterexpress
                };
                #endregion

                #region 包含指定项目的临时物料
                if (MaterialCodes.Any())
                {
                    filterexpress = filterexpress.And(p => MaterialCodes.Contains(p.MaterialCode));
                }

                #endregion
            }



            return (filterexpress, isProductfilter, isSpacefilter, IsInputfilter, IsReplacefilter, partIds);
        }


        #endregion
    }
}
