using System;
// 计算每片瓷砖的面积
    static decimal CalculateTileArea(int lengthMm, int widthMm)
    {
        // 将毫米转换为米
        decimal lengthM = lengthMm / 1000m;
        decimal widthM = widthMm / 1000m;
        return lengthM * widthM;
    }

    // 将平米数转换为片数
    static decimal SquareMetersToTiles(decimal areaInSquareMeters, decimal tileArea)
    {
        return areaInSquareMeters / tileArea;
    }

    // 将片数转换为平米数
    static decimal TilesToSquareMeters(decimal tileCount, decimal tileArea)
    {
        return tileCount * tileArea;
    }

    // 采购价（元/平方米）
        decimal caiGouJia = 80.00m;

       
    
        int tileLengthMm = 1200;  // 瓷砖的长度（毫米）
        int tileWidthMm = 600;    // 瓷砖的宽度（毫米）
        decimal tileArea = CalculateTileArea(tileLengthMm, tileWidthMm);  // 每片瓷砖的面积（平方米）

         // 计算单个瓷砖的价格
        decimal singleTilePrice = caiGouJia * tileArea;
        Console.WriteLine($"单个瓷砖的价格: {singleTilePrice} 元");

        decimal areaInSquareMeters = 5.213m;  // 总面积（平方米）

        // 计算每片瓷砖的面积
        Console.WriteLine($"每片瓷砖的面积: {tileArea} 平方米");

        // 平米转片数
        decimal tilesNeeded = SquareMetersToTiles(areaInSquareMeters, tileArea);
        Console.WriteLine($"需要的瓷砖片数（未四舍五入）: {tilesNeeded} 片");
        Console.WriteLine($"需要的瓷砖片数（四舍五入）: {Math.Round(tilesNeeded)} 片");
        Console.WriteLine($"结算价（四舍五入）: {singleTilePrice*Math.Round(tilesNeeded)} 片");

        // 片数转平米
        decimal totalArea = TilesToSquareMeters(Math.Round(tilesNeeded), tileArea);
        Console.WriteLine($"总面积（四舍五入后）: {totalArea} 平方米");

        // 计算误差
        decimal error = areaInSquareMeters - totalArea;
        Console.WriteLine($"误差: {error} 平方米");
    



