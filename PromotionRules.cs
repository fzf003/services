using System;
using System.Collections.Generic;

// -------------------- 数据结构 --------------------
public class PromotionRules
{
    public string ConditionType { get; set; }
    public string LogicalOperator { get; set; } // 组间逻辑
    public List<PromotionRulesItem> RuleItems { get; set; } = new();
}

public class PromotionRulesItem
{
    public string ConditionKey1 { get; set; }
    public string ConditionKey2 { get; set; }
    public string RelationalOperator { get; set; }
    public string ConditionValue1 { get; set; }
    public string ConditionValue2 { get; set; }
    public string LogicalOperator { get; set; } // 子项逻辑
}

public class PromotionContext
{
    public Dictionary<string, object> Data { get; set; } = new();
}

// -------------------- 条件计算器 --------------------
public interface IConditionEvaluator
{
    bool Evaluate(PromotionRulesItem item, PromotionContext context);
}

public class DefaultConditionEvaluator : IConditionEvaluator
{
    public bool Evaluate(PromotionRulesItem item, PromotionContext context)
    {
        bool EvalSingle(string key, string value)
        {
            if (!context.Data.TryGetValue(key, out var actual))
                return false;

            if (decimal.TryParse(actual?.ToString(), out var actualNum) &&
                decimal.TryParse(value, out var valueNum))
            {
                return item.RelationalOperator switch
                {
                    ">=" => actualNum >= valueNum,
                    "<=" => actualNum <= valueNum,
                    "=" => actualNum == valueNum,
                    _ => throw new NotSupportedException($"不支持运算符: {item.RelationalOperator}")
                };
            }

            return item.RelationalOperator switch
            {
                "=" => actual?.ToString() == value,
                _ => throw new NotSupportedException($"非数值类型不支持运算符: {item.RelationalOperator}")
            };
        }

        // 支持收款条件 + 第二条件类型
        if (item.ConditionKey1 == "收款条件" && !string.IsNullOrEmpty(item.ConditionKey2))
        {
            if (!context.Data.TryGetValue(item.ConditionValue1, out var amountObj))
                return false;
            if (!decimal.TryParse(amountObj?.ToString(), out var actualAmount))
                return false;

            decimal targetValue = 0;

            if (item.ConditionKey2.Equals("固定金额", StringComparison.OrdinalIgnoreCase))
            {
                if (!decimal.TryParse(item.ConditionValue2, out targetValue)) return false;
            }
            else if (item.ConditionKey2.Equals("固定比例", StringComparison.OrdinalIgnoreCase))
            {
                if (!decimal.TryParse(item.ConditionValue2, out var ratio)) return false;
                targetValue = actualAmount * ratio;
            }
            else if (item.ConditionKey2.Equals("指定比例", StringComparison.OrdinalIgnoreCase))
            {
                // 支持百分比字符串，比如 "50%"
                if (!item.ConditionValue2.EndsWith("%")) return false;
                if (!decimal.TryParse(item.ConditionValue2.TrimEnd('%'), out var percent)) return false;
                targetValue = actualAmount * (percent / 100m);
            }
            else
            {
                return EvalSingle(item.ConditionKey2, item.ConditionValue2) && EvalSingle(item.ConditionKey1, item.ConditionValue1);
            }

            Console.WriteLine($"计算: {item.ConditionKey1}={actualAmount}, {item.ConditionKey2}目标={targetValue} => {item.RelationalOperator}");
            return item.RelationalOperator switch
            {
                ">=" => actualAmount >= targetValue,
                "<=" => actualAmount <= targetValue,
                "=" => actualAmount == targetValue,
                _ => throw new NotSupportedException($"不支持运算符: {item.RelationalOperator}")
            };
        }

        // 普通单条件
        bool result1 = EvalSingle(item.ConditionKey1, item.ConditionValue1);
        bool result2 = string.IsNullOrEmpty(item.ConditionKey2) ? true : EvalSingle(item.ConditionKey2, item.ConditionValue2);
        Console.WriteLine($"计算: {item.ConditionKey1}={context.Data[item.ConditionKey1]} {(string.IsNullOrEmpty(item.ConditionKey2) ? "" : $"and {item.ConditionKey2}={context.Data.GetValueOrDefault(item.ConditionKey2)}")} => {result1 && result2}");
        return result1 && result2;
    }
}

// -------------------- 规则计算器 --------------------
public static class PromotionRulesEvaluator
{
    public static bool EvaluateRuleItems(PromotionRules rule, PromotionContext context, IConditionEvaluator evaluator)
    {
        if (rule.RuleItems.Count == 0) return true;

        bool result = evaluator.Evaluate(rule.RuleItems[0], context);

        for (int i = 1; i < rule.RuleItems.Count; i++)
        {
            var item = rule.RuleItems[i];
            bool itemResult = evaluator.Evaluate(item, context);

            result = item.LogicalOperator switch
            {
                "且" => result && itemResult,
                "或" => result || itemResult,
                null or "" => result && itemResult,
                _ => throw new NotSupportedException($"不支持子项逻辑运算符: {item.LogicalOperator}")
            };
        }

        return result;
    }

    public static bool IsSatisfied(IEnumerable<PromotionRules> rules, PromotionContext context, IConditionEvaluator evaluator)
    {
        bool? result = null;

        foreach (var rule in rules)
        {
            bool ruleResult = EvaluateRuleItems(rule, context, evaluator);
            if (result == null) result = ruleResult;
            else
            {
                result = rule.LogicalOperator switch
                {
                    "且" => result.Value && ruleResult,
                    "或" => result.Value || ruleResult,
                    null or "" => result.Value && ruleResult,
                    _ => throw new NotSupportedException($"不支持规则组逻辑运算符: {rule.LogicalOperator}")
                };
            }
        }

        return result ?? true;
    }
}

// -------------------- 测试示例 --------------------
public class Program
{
    public static void Main()
    {
        var context = new PromotionContext
        {
            Data =
            {
                ["设计费"] = 6000,
                ["签约条件"] = "设计协议"
            }
        };

        var rules = new List<PromotionRules>
        {
            new PromotionRules
            {
                ConditionType = "参与条件",
                LogicalOperator = "且",
                RuleItems =
                {
                    // 单条件
                    new PromotionRulesItem
                    {
                        ConditionKey1 = "签约条件",
                        ConditionKey2 = "",
                        RelationalOperator = "=",
                        ConditionValue1 = "设计协议",
                        ConditionValue2 = "",
                        LogicalOperator = "且"
                    },
                    // 固定金额
                    new PromotionRulesItem
                    {
                        ConditionKey1 = "收款条件",
                        ConditionKey2 = "固定金额",
                        RelationalOperator = ">=",
                        ConditionValue1 = "设计费",
                        ConditionValue2 = "5000",
                        LogicalOperator = "且"
                    },
                    // 固定比例
                    new PromotionRulesItem
                    {
                        ConditionKey1 = "收款条件",
                        ConditionKey2 = "固定比例",
                        RelationalOperator = ">=",
                        ConditionValue1 = "设计费",
                        ConditionValue2 = "0.5",
                        LogicalOperator = "且"
                    },
                    // 百分比
                    new PromotionRulesItem
                    {
                        ConditionKey1 = "收款条件",
                        ConditionKey2 = "指定比例",
                        RelationalOperator = ">=",
                        ConditionValue1 = "设计费",
                        ConditionValue2 = "50%",
                        LogicalOperator = "且"
                    }
                }
            }
        };

        var evaluator = new DefaultConditionEvaluator();
        bool result = PromotionRulesEvaluator.IsSatisfied(rules, context, evaluator);
        Console.WriteLine($"最终计算结果: {result}");
    }
}
