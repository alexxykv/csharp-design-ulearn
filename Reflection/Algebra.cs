using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Reflection.Differentiation
{
    public class Algebra
    {
        public static Expression<Func<double, double>> Differentiate(Expression<Func<double, double>> function)
        {
            return Expression.Lambda<Func<double, double>>(
                Differentiate(function.Body),
                function.Parameters
                );
        }

        public static Expression Differentiate(Expression expression)
        {
            var visitor = new DifferentiateVisitor();
            return visitor.Visit(expression);
        }
    }

    public class DifferentiateVisitor : ExpressionVisitor
    {
        static readonly MethodInfo Cos;
        static readonly MethodInfo Sin;

        static DifferentiateVisitor()
        {
            Cos = typeof(Math).GetMethod("Cos", new[] { typeof(double) });
            Sin = typeof(Math).GetMethod("Sin", new[] { typeof(double) });
        }

        protected override Expression VisitConstant(ConstantExpression expression)
        {
            return Expression.Constant(0.0);
        }

        protected override Expression VisitParameter(ParameterExpression expression)
        {
            return Expression.Constant(1.0);
        }

        protected override Expression VisitBinary(BinaryExpression expression)
        {
            if (expression.NodeType == ExpressionType.Add)
            {
                return Expression.Add(
                    Algebra.Differentiate(expression.Left),
                    Algebra.Differentiate(expression.Right)
                    );
            }

            if (expression.NodeType == ExpressionType.Multiply)
            {
                return Expression.Add(
                    Expression.Multiply(
                        Algebra.Differentiate(expression.Left),
                        expression.Right
                        ),
                    Expression.Multiply(
                        expression.Left,
                        Algebra.Differentiate(expression.Right)
                        )
                    );
            }

            throw new ArgumentException($"{expression.NodeType} operation is not supported");
        }

        protected override Expression VisitMethodCall(MethodCallExpression expression)
        {
            if (expression.Method == Sin)
            {
                return Expression.Multiply(
                    Expression.Call(
                        Cos,
                        expression.Arguments
                    ),
                    Algebra.Differentiate(expression.Arguments[0])
                );
            }

            if (expression.Method == Cos)
            {
                return Expression.Multiply(
                    Expression.Negate(
                        Expression.Call(
                            Sin,
                            expression.Arguments
                            )
                        ),
                    Algebra.Differentiate(expression.Arguments[0])
                    );
            }

            throw new ArgumentException($"{expression.Method.Name} method is not supported");
        }
    }
}
