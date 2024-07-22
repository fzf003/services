
{
    "大类":"",
    "子类":"",
    "单位":"",
    "结算信息": [{
        "分公司": [{
            "CompanyCode": 2193,
            "CompanyName": "北京分公司"
        }, {
            "CompanyCode": 2320,
            "CompanyName": "郑州分公司"
        }],
       
        "结算轮次": [{
                "index": 1,
                "First": "开工",
                "Ratio": 10
            }, {
                "index": 2,
                "Second": "隐蔽",
                "Ratio": 10
            }, {
                "index": 3,
                "Third": "调试",
                "Ratio": 30
            },
            {
                "index": 5,
                "Fifth": "交付",
                "Ratio": 10
            }]
    }],
    "SetUnit": [
        { "UnitName": "个" },
        { "UnitName": "副" },
        { "UnitName": "位" },
    ],
    "SetPrices":[
        {"CompanyId":2193, "CompanyName": "北京分公司","Unit":"900","Villa":"900"},
        {"CompanyId":2320, "CompanyName": "郑州分公司","Unit":"900","Villa":"900"},
        {"CompanyId":2194, "CompanyName": "成都分公司","Unit":"900","Villa":"900"}
    ]
}
/********************************************************************************************************/

{
    "processName": "千选类目配置",
    "actionName": "提交",
    "stepID": -1,
    "taskID": -1,
    "subModel": "POST",
    "comment": "",
    "aspxauth": "",
    "token": "",
    "CategoryConfig": {
        "PtName": "空调",
        "PtCode": "T023",
        "PtiName": "氟机112",
        "PtiCode": "T023TT71",
        "SettleNodes": [
            {
                "TrunCode": 1,
                "BranchOffices": [
                    {
                        "CompanyCode": 2193,
                        "CompanyName": "北京分公司"
                    },
                    {
                        "CompanyCode": 2320,
                        "CompanyName": "郑州分公司"
                    }
                ],
                "SettlePeriods": [
                    {
                        "TotalPeriodCode": 1,
                        "TotalPeriodName": "开工",
                        "SettRatio": 20,
                        "Indexx": 1
                    },
                    {
                        "TotalPeriodCode": 2,
                        "TotalPeriodName": "隐蔽",
                        "SettRatio": 10,
                        "Indexx": 2
                    },
                    {
                        "TotalPeriodCode": 3,
                        "TotalPeriodName": "调试",
                        "SettRatio": 30,
                        "Indexx": 3
                    },
                    {
                        "TotalPeriodCode": 4,
                        "TotalPeriodName": "交付",
                        "SettRatio": 40,
                        "Indexx": 4
                    }
                ]
            }
        ],
        "SetUnit": {
            "UnitName": "个"
        },
        "SetPrices": [
            {
                "CompanyCode": 2193,
                "CompanyName": "北京分公司",
                "UnitPrice": 900,
                "VillaPrice": 900
            },
            {
                "CompanyCode": 2320,
                "CompanyName": "郑州分公司",
                "UnitPrice": 900,
                "VillaPrice": 900
            },
            {
                "CompanyCode": 2194,
                "CompanyName": "成都分公司",
                "UnitPrice": 900,
                "VillaPrice": 900
            }
        ],
        "SetSTProducts": [
            {
                "STProName": "001"
            }
        ]
    }
}
