
        static int Amount = 0;

        static int OrignAmount=0;
        static void Main(string[] args)
        {
             
            List<Step> Setps = new List<Step>
            {
                new Step { Min = 0, Max = 1000, discount = 8 },
                new Step { Min = 1000, Max = 5000, discount = 7.5 },
                new Step { Min = 5000, Max = 10000, discount = 6 },
                  new Step { Min = 10000, Max = 30000, discount = 5.3 }
            };

             decimal valueToMatch = 120900;
                  decimal totalDiscount = 0;

                 Console.WriteLine($"分拆金额: {valueToMatch}");
          

            totalDiscount=  CalculateDiscount(Setps, valueToMatch);

 Console.WriteLine($"总折扣金额: {totalDiscount}");

 Console.WriteLine($"总剩余金额: {initialValueToMatch-totalDiscount}");

 Console.WriteLine($"验证:{totalDiscount+(initialValueToMatch - totalDiscount)}");




            Console.WriteLine();

            Console.ReadKey();




        }

 static decimal CalculateDiscount(List<Step> steps, decimal valueToMatch)
 {
     decimal totalDiscount = 0;

     foreach (var step in steps)
     {
         if (valueToMatch <= 0)
         {
             break;
         }

         decimal stepValue = Math.Min(valueToMatch, step.Max);
         Console.WriteLine($"Curr:{valueToMatch}--Max:{step.Max}---{stepValue}");
         decimal stepDiscount = stepValue * (step.discount / 10);
         totalDiscount += stepDiscount;

         valueToMatch -= stepValue;
         Console.WriteLine($"区间：Min: {step.Min}, Max: {step.Max}, 折扣: {stepDiscount} 剩余:{valueToMatch}");

     }

     return totalDiscount;
 }

 public class Step
 {
     public decimal Min { get; set; }
     public decimal Max { get; set; }
     public decimal discount { get; set; }
 };

        static void PrintMatchingSteps(List<Step> steps, int valueToMatch, int startIndex = 0)
        {
            if (startIndex >= steps.Count)
            {
               // return;
            }

            var otherSteps = steps;

            var maxStep = otherSteps.Aggregate((max, step) => step.Max > max.Max ? step : max);
  
            if ((valueToMatch >= maxStep.Max) || (valueToMatch > maxStep.Min && valueToMatch <= maxStep.Max)) ///匹配到此区间
            {
                ///valueToMatch = (valueToMatch - maxStep.Max);

                var c=  Convert.ToInt32((valueToMatch / maxStep.Max));
                valueToMatch = valueToMatch - (c * maxStep.Max);
                Amount += (c * maxStep.Max);
                Console.WriteLine($"打折金额:{c*maxStep.Max} 第一次匹配：Min: {maxStep.Min}, Max: {maxStep.Max}, Discount: {maxStep.discount}");

                PrintMatchingSteps(steps, valueToMatch, startIndex + 1);
            }
            else
            {
                if (valueToMatch > 0)
                {
                    otherSteps = otherSteps.Where(step => step != maxStep).ToList();
                    var currentStep = otherSteps[startIndex];

                    if (valueToMatch > currentStep.Min && valueToMatch <= currentStep.Max)
                    {
                        Console.WriteLine($"余额:{valueToMatch} 区域：Min: {currentStep.Min}, Max: {currentStep.Max}, Discount: {currentStep.discount}");


                        var c = Convert.ToInt32((valueToMatch / currentStep.Max));

                        valueToMatch= valueToMatch - (c * currentStep.Max);

                        Amount += (c * currentStep.Max);

                        if (Amount != OrignAmount)
                        {
                            PrintMatchingSteps(otherSteps, valueToMatch, startIndex + 1);
                        }
                    }
                }


                if (Amount == OrignAmount)
                {
                    Console.WriteLine($"剩余不打折: {OrignAmount - Amount}");
                }

            }
