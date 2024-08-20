视图:
V_GZ_ProductLineView
V_GZ_ProductLineViewForNew
V_GZ_ProductLineViewForNew_OrderbyFrequency


---老定额信息
V_DetailMSG
V_DetailMSG_ForVersionCode


-----Proc-------------------
--老定额
PROC_Change_DetailMSGForVersionCode
PROC_Change_DetailMSG


PROC_PM_Price_ActivateCustomQuota
PROC_QueryPM_Base_QuotaPrice(SCZSPMDATA)
----------------------------

ESB:是否开通自定义定额、变更单产品线个性化定额。

页面：

施工变更单  报价审核     自定义定额信息

 <!--https://testbpm.voglassdc.com/bpm/YZSoft/Forms/XForm/JZForm/PM/自定义定额信息.aspx--> 

yx:
SCZS.YX.BPMCOREServices/IServices/IQuotePriceService.cs
SCZS.YX.BPMCOREServices/Services/QuotePriceService.cs
SCZS.YX.BPMCORE/Controllers/CustomerProductLineController.cs
SCZS.YX.BPMCOREModel/Filter/QuotePrice/GetIsActivateCustomQuotaFilter.cs

DRTPMCore：

SCZS.DRT_PMCore.Services/Quota/QuotaService.cs


SCZS.DRT_PMCore.Api


1. 超退数量:就是退货超出下单的数量。同一部位相同物料下单数量-退货数量<0 就是超退，具体超几个看此处数据，0表示没有超，只有历史数据有超退的。
2. 可退补数量:就是退货剩余数量。同一部位相同物料下单数量-退货数量=可退补数量 。
3. 原升级小计: 销售方式为升级物料 升级小计=升级价*数量。在退补框中选择的，有原升级小计。 其他框子中选择为0。 （PS：不需要太关注）
4. 原单独合计：单独销售方式物料。单独销售合计=单独销售价*数量。 规则同原升级小计。 （PS：不需要太关注）
5. 变更升级小计：销售方式为升级物料 此次变更物料的合计。
6. 活动变更升级小计：销售方式为升级物料并且是活动物料的此次变更物料的合计。
7. 单独合计: 单独销售方式物料 此次变更物料的合计。
8. 活动单独合计:销售方式为升级物料并且是活动物料的，此次变更物料的合计。
