 public async Task<Output<InitiationProcessOutput>> SubmitProductChangeRelatedProduct(SubmitProductLinesChangeRelateProductInput changeInput)
 {
     Output<InitiationProcessOutput> output = new Output<InitiationProcessOutput>();
     output.IsSuccess = true;

     SCM_Process_ProductLineChange_M ProductLineChangeMain = default;

     var vaildate = await ValidateSubmitActive(changeInput, ProductLineChangeMain);
    
     if (!vaildate.IsSuccess)
     {
         return vaildate;
     }

     InitiationProcessInput processInput = await _commonService.GetProcessInfo(changeInput);

     if (changeInput.StepID > 0)//退回重填
     {
        var prodctlinechangeinfo= await this._productLineManager.GetProcessProductLineChangeInfo(changeInput.TaskID);
         if(prodctlinechangeinfo is null)
         {
             output.IsSuccess = false;
             output.Message = "当前变更单不存在";
             return output;
         }
         ProductLineChangeMain.ID= prodctlinechangeinfo.ID;
         ProductLineChangeMain.TaskID= prodctlinechangeinfo.TaskID;
     }

    
     List<SCM_Process_ProductLineChange_M> ProcessMain = new List<SCM_Process_ProductLineChange_M>()
     {
        ProductLineChangeMain
     };

     processInput.FormData.Add(BPMCoreMiddleWare.ProcessService.ModelToFormData<SCM_Process_ProductLineChange_M>("SCM_Process_ProductLineChange_M", ProcessMain));
     
     IList<SCM_Process_ProductLineChange_SpaceAndPart> Spaces = new List<SCM_Process_ProductLineChange_SpaceAndPart>();
     IList<SCM_Process_ProductLineChange_AddProduct> addproducts = new List<SCM_Process_ProductLineChange_AddProduct>();
     IList<SCM_Process_ProductLineChange_DelProduct> delproducts = new List<SCM_Process_ProductLineChange_DelProduct>();
     List<SCM_Process_ProductLineChange_AddServiceBag> addpackage = new List<SCM_Process_ProductLineChange_AddServiceBag>();
     List<SCM_Process_ProductLineChange_DelServiceBag> delpackage = new List<SCM_Process_ProductLineChange_DelServiceBag>();

     int relationIndex = 1;
     int i = 1;
     int k= 1;

     if (changeInput.SpaceAndParts.Any())
     {
         foreach (var spaceitem in changeInput.SpaceAndParts)
         {
             spaceitem.RelationRowGuid = relationIndex;
          
             Spaces.Add(this.Mapper.Map<SCM_Process_ProductLineChange_SpaceAndPart>(spaceitem));

             if (changeInput.AddProducts.Any())
             {
                 foreach (var addproduct in changeInput.AddProducts)
                 {
                     addproduct.RelationRowGuid = i;
                     addproduct.RelationParentRowGuid = spaceitem.RelationRowGuid;
                     addproducts.Add(this.Mapper.Map<SCM_Process_ProductLineChange_AddProduct>(addproduct));
                 }
 
             }

             if (changeInput.DelProducts.Any())
             {
                 foreach (var delproduct in changeInput.DelProducts)
                 {
                     delproduct.RelationRowGuid = k;
                     delproduct.RelationParentRowGuid = spaceitem.RelationRowGuid;
                     delproducts.Add(this.Mapper.Map<SCM_Process_ProductLineChange_DelProduct>(delproduct));
                 }
             }
         }
      }

     if(changeInput.AddServicePackages.Any())
     {
         addpackage.AddRange(this.Mapper.Map<List<SCM_Process_ProductLineChange_AddServiceBag>>(changeInput.AddServicePackages));
     }

     if(changeInput.DelServicePackages.Any())
     {
         delpackage.AddRange(this.Mapper.Map<List<SCM_Process_ProductLineChange_DelServiceBag>>(changeInput.DelServicePackages));
     }

     processInput.FormData = new List<FormData>
         {
                 
                 BPMCoreMiddleWare.ProcessService.ModelToFormData<SCM_Process_ProductLineChange_SpaceAndPart>("SCM_Process_ProductLineChange_SpaceAndPart", Spaces),
                 BPMCoreMiddleWare.ProcessService.ModelToFormData<SCM_Process_ProductLineChange_AddProduct>("SCM_Process_ProductLineChange_AddProduct", addproducts),
                 BPMCoreMiddleWare.ProcessService.ModelToFormData<SCM_Process_ProductLineChange_DelProduct>("SCM_Process_ProductLineChange_DelProduct", delproducts),
                 BPMCoreMiddleWare.ProcessService.ModelToFormData<SCM_Process_ProductLineChange_AddServiceBag>("SCM_Process_ProductLineChange_AddServiceBag", addpackage),
                 BPMCoreMiddleWare.ProcessService.ModelToFormData<SCM_Process_ProductLineChange_DelServiceBag>("SCM_Process_ProductLineChange_DelServiceBag", delpackage)
         };

     var result = await BPMCoreMiddleWare.ProcessService.InitiationProcess(processInput);
     output.IsSuccess = result.IsSuccess;
     output.Message = result.Message;
     output.Data = result.data;
     return output;

 }
 /// <summary>
 /// 提交参数验证
 /// </summary>
 /// <param name="request"></param>
 /// <returns></returns>
 async Task<Output<InitiationProcessOutput>> ValidateSubmitActive(SubmitProductLinesChangeRelateProductInput request, SCM_Process_ProductLineChange_M productLineChange)
 {
     Output<InitiationProcessOutput> output = new Output<InitiationProcessOutput>();
     output.IsSuccess = true;

     var processmian = request.ProductLineChangeMain;
     processmian.CreateCompanyCode = LoginUserInfo.CompanyCode;
     processmian.CreateCompanyName = LoginUserInfo.CompanyName;
     processmian.CreateUser = LoginUserInfo.DisplayName;
     processmian.CreateUserAccount = LoginUserInfo.Account;
     processmian.DeptName = LoginUserInfo.DeptName;
     processmian.DeptCode = LoginUserInfo.DeptCode.ToString();
     processmian.CreateTime = DateTime.Now;
     processmian.ModifyType = request.ProcessName;

     productLineChange = this.Mapper.Map<SCM_Process_ProductLineChange_M>(processmian);

     if (!string.IsNullOrWhiteSpace(processmian.ProductLineCode))
     {
         output.Message = "产品线编码不能为空";
         output.IsSuccess = false;
         return output;
     }

     var productlineinfo = await _productLineManager.GetProductLineByCode(processmian.ProductLineCode);

     if (productlineinfo is null)
     {
         output.Message = $"该产品线编码:{processmian.ProductLineCode},不存在";
         output.IsSuccess = false;
         return output;
     }

     if (request.SpaceAndParts is null)
     {
         output.Message = "提交的空间信息提交数据格式不对";
         output.IsSuccess = false;
         return output;
     }

     if (request.AddProducts is null)
     {
         output.Message = "新增的产品信息提交数据格式不对";
         output.IsSuccess = false;
         return output;
     }

     if (request.DelProducts is null)
     {
         output.Message = "取消的产品信息提交数据格式不对";
         output.IsSuccess = false;
         return output;
     }

     if (request.AddServicePackages is null)
     {
         output.Message = "添加的服务包信息提交数据格式不对";
         output.IsSuccess = false;
         return output;
     }

     if (request.DelServicePackages is null)
     {
         output.Message = "取消的服务包信息提交数据格式不对";
         output.IsSuccess = false;
         return output;
     }

     

     var productLineChangeMain = await _productLineManager.GetProductLineChangeVerifyInfo(productlineinfo.ProductLineCode, "关联产品变更", 0);
     if (productLineChangeMain is not null)
     {
         output.IsSuccess = false;
         output.Message = $"该产品线【关联产品变更】已存在在途单据，在途单号：" + productLineChangeMain.SN;
         return output;
     }


     return output;
 }
