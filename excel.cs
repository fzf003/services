 [HttpPost("upload")]
 public async Task<IActionResult> Upload(IFormFile file)
 {
     if (file == null || file.Length == 0)
         return BadRequest("No file uploaded.");

     var path = Path.Combine(Directory.GetCurrentDirectory(), file.FileName);

     using (var stream = new FileStream(path, FileMode.Create))
     {
         await file.CopyToAsync(stream);
     }

     IWorkbook workbook = new XSSFWorkbook(path);
     ISheet sheet = workbook.GetSheetAt(0); // 获取第一个工作表

     for (int row = 0; row <= sheet.LastRowNum; row++)
     {
         if (sheet.GetRow(row) != null) // Null 表示该行没有数据
         {
             for (int col = 0; col < sheet.GetRow(row).LastCellNum; col++)
             {
                 Console.Write(sheet.GetRow(row).GetCell(col)?.ToString() + "\t");
             }
             Console.WriteLine();
         }
     }

   

     workbook.Close();




     return Ok(new { file.FileName, file.Length });
 }
