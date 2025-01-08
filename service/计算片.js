
let INum = 5.213;
let LongSize = 1200; // 瓷砖的长度（毫米）
let WidthSize = 600; // 瓷砖的宽度（毫米）
let caiGouJia = 80.00; // 采购价格（单位：元）

// 计算每片瓷砖的面积（平方米）
let tileArea = (LongSize / 1000) * (WidthSize / 1000); 
console.log(`每片瓷砖的面积: ${tileArea.toFixed(4)} 平方米`);

// 转换采购数量（单位：片）
let convertcaigounum = (INum * 1000000.00) / (LongSize * WidthSize); 
console.log(`转换采购数量（未四舍五入）: ${convertcaigounum}`);

convertcaigounum = convertcaigounum.toFixed(2);
console.log(`转换采购数量（四舍五入）: ${convertcaigounum}`);

// 实际采购数量（单位：片）
let shijiNum = INum < 0 ? Math.ceil(convertcaigounum) - 1 : Math.ceil(convertcaigounum);
console.log(`实际采购数量: ${shijiNum}`);

// 实际采购金额（结算金额）
let casetotal = (caiGouJia * LongSize * WidthSize / 1000000.00) * shijiNum;
console.log(`实际采购金额（结算金额）: ${casetotal.toFixed(2)} 元`);

// 转换后的采购价格
let convertPrice = (casetotal / shijiNum).toFixed(2);
console.log(`转换后的采购价格: ${convertPrice} 元`);

// 剩余数量（单位：片）
let shengyunum = (shijiNum - convertcaigounum).toFixed(2);
console.log(`剩余数量: ${shengyunum} 片`);

// 转化后金额补充
let BuchongshijiCaigou = (convertPrice * shengyunum).toFixed(2);
console.log(`转化后金额补充: ${BuchongshijiCaigou} 元`);
===============================================================================================================================================================================
let INum =-5
 

let LongSize =1200.00

let WidthSize = 600

let caiGouJia =80.00

//64.80  61.20
 

let convertcaigounum = (INum * 1000000.00) / (LongSize * WidthSize);// 单位：片 转换采购数量
console.log(convertcaigounum);

convertcaigounum = convertcaigounum.toFixed(2);

console.log(`转换采购数量:${convertcaigounum}`);

let shijiNum = INum < 0 ? Math.ceil(convertcaigounum) - 1 : Math.ceil(convertcaigounum);// 单位:片 向上取整

console.log(`实际采购数量:${shijiNum}`);

let casetotal = (caiGouJia * LongSize * WidthSize / 1000000.00) * shijiNum; //实际采购金额（结算金额）

console.log(`实际采购金额（结算金额）:${casetotal}`);

let convertPrice = (casetotal / shijiNum).toFixed(2);///转换后的采购价格
console.log(`转换后的采购价格:${convertPrice}`);

let shengyunum = (shijiNum - convertcaigounum).toFixed(2);// 单位：片 剩余数量

console.log(`剩余数量:${shengyunum}`);

let BuchongshijiCaigou = (convertPrice * shengyunum).toFixed(2);///转化后金额补充

console.log(`转化后金额补充:${BuchongshijiCaigou}`);

console.log(-6-1);



/*function zhuanhuadanwei(longsize, widthsize, num, iszh) {
    if (iszh == "是") {
        let m = ((num * 1000000) / (longsize * widthsize)).toFixed(2);
        return m < 0 ? Math.ceil(m) - 1 : Math.ceil(m);
    }
    else {
        return num;
    }
}*/




//SCYXDATA:PM_Price_Change_T.convertcaigounum
function zhuanhuadanwei(longsize, widthsize, num, iszh) {
    if (iszh == "是") {


        return Math.ceil((num * 1000000) / (longsize * widthsize));
    }
    else {
        return num;
    }
}

//SCYXDATA:PM_Price_Change_T.OldNum
function zhuanhuadanwei(longsize, widthsize, num, iszh, TReturnNum, OldNum) {
    if (iszh == "是") {

        let convertcaigounum = ((num * 1000000) / (longsize * widthsize)).toFixed(2);

        let currshijiNum = num < 0 ? Math.ceil(convertcaigounum) - 1 : Math.ceil(convertcaigounum);// 单位:片 向上取整

        let oldshijinum = Math.ceil(((OldNum * 1000000) / (longsize * widthsize)).toFixed(2));//原始数量

        let returnshijinum = Math.ceil(((TReturnNum * 1000000) / (longsize * widthsize)).toFixed(2));//可退补数量

        ///如果退补时数量为负数,可退补数量为0就表示,数量已退完。
        ///则获取该物料变更数量累计  (可退补数量-(已在页面上的信息数量))

        if (num < 0) ///退货
        {
            if(OldNum=num)///全退
            {
              return currshijiNum;
            }
            
            
            if (TReturnNum == 0) {
                return returnshijinum;
            }
        }
 
        return currshijiNum;
    }
    else {
        return num;
    }
}



/*function getConvertCaigoujiaTotal(activeprice, caigoujia, longsize, widthsize, num, iszh) {
    if (iszh == "是") {

        let currnum = (num * 1000000) / (longsize * widthsize).toFixed(2);

        var shijinum = currnum < 0 ? Math.ceil(currnum) - 1 : Math.ceil(currnum);

        if (activeprice > 0) return activeprice * longsize * widthsize / 1000000 * shijinum;
        else return caigoujia * longsize * widthsize / 1000000 * shijinum;
    }
    else {
        if (activeprice > 0) return activeprice * num;
        return caigoujia * num;
    }
}*/



///SCYXDATA:PM_Price_Change_T.ChangeCaiGouJiaTotal
function getConvertCaigoujiaTotal(activeprice, caigoujia, longsize, widthsize, num, iszh) {
    if (iszh == "是") {

        let currnum = (num * 1000000) / (longsize * widthsize).toFixed(2);

        var shijinum = currnum < 0 ? Math.ceil(currnum) - 1 : Math.ceil(currnum);

        if (activeprice > 0) return activeprice * longsize * widthsize / 1000000 * shijinum;
        else return caigoujia * longsize * widthsize / 1000000 * shijinum;
    }
    else {
        if (activeprice > 0) return activeprice * num;
        return caigoujia * num;
    }
}

let ppp = getConvertCaigoujiaTotal(0, caiGouJia, LongSize, WidthSize, INum, "是");

console.log(`转换后:${ppp}`);

let rrr = zhuanhuadanwei(LongSize, WidthSize, INum, "是");

console.log(`转换单位:${rrr}`);









/*
 convertcaigounum = (_Update.Num * 1000000.00M) / (LongSize * WidthSize); //有小数 单位：片
                        convertcaigounum = Math.Round(convertcaigounum, 2);
                        shijiNum = Math.Ceiling(convertcaigounum); //取整数 单位：片
                        casetotal = (caiGouJia * LongSize * WidthSize / 1000000.00M) * shijiNum; //实际采购金额（结算金额）
                        convertPrice = Math.Round(casetotal / shijiNum, 2);

                        shengyunum = shijiNum - convertcaigounum;

                        //转化后金额补充
                        BuchongshijiCaigou = Math.Round(convertPrice * shengyunum, 2);*/
