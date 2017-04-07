﻿using System;
using static LoxLanguage.Expression;

namespace LoxLanguage
{
    public class Interpreter : IVisitor<object>
    {
        public object Visit(Literal literal)
        {
            return literal.value;
        }

        public object Visit(Postfix postfix)
        {
            throw new NotImplementedException();
        }

        public object Visit(Conditional conditional)
        {
            throw new NotImplementedException();
        }

        public object Visit(Prefix prefix)
        {
            object right = Evaluate(prefix.rhs);

            switch(prefix.opp.type)
            {
                case TokenType.Bang:
                    return !IsTrue(right);
                case TokenType.Minus:
                    return -(double)right;
            }

            // Unreachable
            return null;
        }


        public object Visit(Grouping grouping)
        {
            return Evaluate(grouping.expression);
        }

        public object Visit(Binary binary)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks if a value is true or not in Lox. Null is false and a boolean
        /// returns it's value. Everything else is true. 
        /// </summary>
        private bool IsTrue(object value)
        {
            if (value == null) return false;
            if (value is bool) return (bool)value;
            return true; 
        }

        /// <summary>
        /// Returns the value of the expression.
        /// </summary>
        private object Evaluate(Expression expression)
        {
            return expression.Accept(this);
        }
    }
}
