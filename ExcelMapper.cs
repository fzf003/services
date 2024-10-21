var filePath = "户型配置模板.xlsx";

var products = new ExcelMapper(filePath)
{
    HeaderRow = true,
    HeaderRowNumber = 1,
    MinRowNumber = 1
};


var list = products.Fetch<HouseBaseConfigTemplate>(sheetIndex: 0);

foreach (var item in list)
{
    Console.WriteLine(item);
}

var outputmapper=new ExcelMapper();

outputmapper.Saving += (s, e) =>
{

    Console.WriteLine(e.Sheet.LastRowNum);
    var sheet = e.Sheet;
    var Workbook = sheet.Workbook;
   
    sheet.ShiftRows(0, sheet.LastRowNum, 1);  // 向下移动一行
    IRow titleRow = sheet.CreateRow(0);       // 创建新的第一行
    ICell titleCell = titleRow.CreateCell(0); // 在第一行创建一个单元格
    titleCell.SetCellValue("设置的Title"); // 设置单元格的内容
                                        // 合并单元格（可选，如果希望标题跨多列）
    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, 19)); 


    ICellStyle titleStyle = Workbook.CreateCellStyle();
    IFont font = Workbook.CreateFont();
    font.IsBold = true;  // 字体加粗
    font.FontHeightInPoints = 16;  // 字号16
    titleStyle.SetFont(font);

    // 设置居中对齐
    titleStyle.Alignment = HorizontalAlignment.Center;  // 水平居中
    titleStyle.VerticalAlignment = VerticalAlignment.Center;
    titleCell.CellStyle = titleStyle;



};

await outputmapper.SaveAsync("products1.xlsx", list, "Products");
