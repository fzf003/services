
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

            const int valueToMatch = 120900;
            OrignAmount = valueToMatch;

            PrintMatchingSteps(Setps, valueToMatch);






            Console.WriteLine();

            Console.ReadKey();




        }

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
