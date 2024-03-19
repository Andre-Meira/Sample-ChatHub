using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Sample.ChatHub.Domain.Abstracts;

public static class ObjectHelper
{
    public static bool IsAny(this object o, params Type[] types)
    {
        return types.Contains(o.GetType());
    }
}

public class ParameterReplacer : ExpressionVisitor
{
    private readonly ParameterExpression _oldParameter;
    private readonly ParameterExpression _newParameter;

    public ParameterReplacer(ParameterExpression oldParameter, ParameterExpression newParameter)
    {
        _oldParameter = oldParameter;
        _newParameter = newParameter;
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
        return node == _oldParameter ? _newParameter : base.VisitParameter(node);
    }
}

public static class ExpressionHelper
{
    public static Expression<Func<TNewParameter, TResult>> ChangeParameter<TOriginalParameter, TNewParameter, TResult>(Expression<Func<TOriginalParameter, TResult>> expression)
    {
        var oldParameter = expression.Parameters[0];
        var newParameter = Expression.Parameter(typeof(TNewParameter), oldParameter.Name);
        var visitor = new ParameterReplacer(oldParameter, newParameter);
        var newBody = visitor.Visit(expression.Body);
        return Expression.Lambda<Func<TNewParameter, TResult>>(newBody, newParameter);
    }
}