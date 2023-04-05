using System;
using System.Linq.Expressions;

namespace Forum.Model
{
	/// <summary>
	/// Helper to combine predicates in LINQ-to-Entities (SQL).
	/// To be used in `IQueryable` collections.
	/// For reference see: https://www.c-sharpcorner.com/UploadFile/04fe4a/predicate-combinators-in-linq/
	/// Also: https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/expression-trees/
	/// And: https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/expression-trees/how-to-use-expression-trees-to-build-dynamic-queries
	/// As well as: https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/expression-trees/how-to-modify-expression-trees
	/// </summary>
	public static class PredicateBuilderLinq
	{
		/* NOTE: `Func<T, bool>` is a delegate that describes a function which takes a parameter `T` and returns
		 * a `bool`. `Expression<...>` wraps this delegate and stores it into a data tree, which is an
		 * in-memory representation of the lambda expression. This ability to treat expressions as data structures
		 * enables APIs such as LINQ-to-SQL (`IQueryable`) to receive user code that can be inspected, transformed
		 * and processed in a custom manner, like translating an expression tree into an SQL statement that can
		 * be evaluated by the database.
		 * https://docs.microsoft.com/en-us/dotnet/api/System.Linq.Expressions.Expression-1
		 */

		/// <summary>
		/// Default to `true`.
		/// </summary>
		/// <typeparam name="T">Type of object to compare.</typeparam>
		/// <returns>
		/// An in-memory representation of a delegate that describes a function which takes a value `T` and returns a
		/// `bool`, stored in an expression tree.
		/// </returns>
		public static Expression<Func<T, bool>> True<T>()
		{
			return t => true; // returns a function that takes a parameter of type `T` and always returns `true`.
		}

		/// <summary>
		/// Default to `false`.
		/// </summary>
		/// <typeparam name="T">Type of object to compare.</typeparam>
		/// <returns>
		/// An in-memory representation of a delegate that describes a function which takes a value `T` and returns a
		/// `bool`, stored in an expression tree.
		/// </returns>
		public static Expression<Func<T, bool>> False<T>()
		{
			return t => false; // returns a function that takes a parameter of type `T` and always returns `false`.
		}

		/// <summary>
		/// Replace the parameter in the rhs with the parameter from the lhs.
		/// </summary>
		/// <param name="left">Left-hand side operator.</param>
		/// <param name="right">Right-hand side operator.</param>
		/// <typeparam name="T">Type of the object to compare.</typeparam>
		/// <typeparam name="TResult">Result that the expression has to return.</typeparam>
		/// <returns>
		/// The right-hand side predicate as an in-memory representation of a delegate that describes a function which
		/// takes a value of type `T` and returns a result of type `TResult`, stored in a expression tree, with the
		/// first parameter replaced by the lhs-parameter.
		/// </returns>
		private static Expression<Func<TResult>> WithParametersOf<T, TResult>(this Expression<Func<T, TResult>> left,
			Expression<Func<T, TResult>> right)
		{
			// NOTE: Attention!!, we just switched around lhs and rhs.
			// 1. We receive the parameters:
			// lhs = t2 => t2.Tags.Contains(tag)
			// rhs = t1 => false
			// 2. We pass t2 to `ReplaceParameterVisitor._parameter` and t1 to `ReplaceParameterVisitor._replacement`.
			// 3. We pass `t2 => t2.Tags.Contains(tag)` to `ReplaceParameterVisitor.Visit`.
			// 4. We return `() => t1.Tags.Contains(tags)`
			return new ReplaceParameterVisitor<Func<TResult>>(left.Parameters[0], right.Parameters[0]).Visit(left);
		}

		/// <summary>
		/// Predicate `and` comparison.
		/// </summary>
		/// <param name="left">Left-hand side operator.</param>
		/// <param name="right">Right-hand side operator.</param>
		/// <typeparam name="T">Type of the object to compare.</typeparam>
		/// <returns>
		/// An in-memory representation of a delegate that describes a function which takes a value of type `T` and
		/// returns a `bool`, stored in an expression tree.
		/// </returns>
		public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left,
			Expression<Func<T, bool>> right)
		{
			// NOTE: `WithParametersOf` replaces the duplicate parameter in the rhs expression with the
			// parameter of the lhs expression.
			return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left.Body, right.WithParametersOf(left).Body),
				left.Parameters);
		}

		/// <summary>
		/// Predicate `or` comparison.
		/// </summary>
		/// <param name="left">Left-hand side operator.</param>
		/// <param name="right">Right-hand side operator.</param>
		/// <typeparam name="T">Type of the object to compare.</typeparam>
		/// <returns>
		/// An in-memory representation of a delegate that describes a function which takes a value of type`T` and
		/// returns a `bool`, stored in an expression tree.
		/// </returns>
		public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> left,
			Expression<Func<T, bool>> right)
		{
			// NOTE: `WithParametersOf` replaces the duplicate parameter in the rhs expression with the
			// parameter of the lhs expression. (We want to add two expression together, both take a _different_
			// parameter t (t1 and t2). In the combined expression, t1 should be the exact same as t2; we only
			// want to use t1 throughout the whole combined expression.)
			// Example:
			// Would we simply add both expressions together, we end up with an expression that uses two
			// different parameters:
			// t1 => false + t2 => t2.Tags.Contains(tag) results in:
			// t1 => false OrElse t2.Tags.Contains(tag)
			// which is an invalid expression.
			//
			// Instead, we need to replace t2 with t1:
			// lhs: t1 => false
			// rhs: t2 => t2.Tags.Contains(tag)
			// Replace t2 with t1, and remove the duplicate declaration:
			// rhs: () => t1.Tags.Contains(tag)
			// Add both together:
			// t1 => false + () => t1.Tags.Contains(tag)
			// t1 => false OrElse t1.Tags.Contains(tag)
			return Expression.Lambda<Func<T, bool>>(Expression.OrElse(left.Body, right.WithParametersOf(left).Body),
				left.Parameters);
		}

		/// <summary>
		/// Predicate `not` unary comparison. Works like `if (!predicate)`.
		/// </summary>
		/// <param name="left">Left-hand side operator.</param>
		/// <typeparam name="T">Type of the object to compare.</typeparam>
		/// <returns>
		/// An in-memory representation of a delegate that describes a function which takes a value of type`T` and
		/// returns a `bool`, stored in an expression tree.
		/// </returns>
		public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> left)
		{
			// Compile creates a `Func<>` from our `Expression<Func<>>`
			// var compiled = left.Compile();
			// return t => !compiled(t);
			// NOTE: But we require it to stay in form of an expression, to use it together with LINQ-to-SQL.
			
			return Expression.Lambda<Func<T, bool>>(Expression.Not(left));
		}

		/// <summary>
		/// Predicate `or` comparison.
		/// </summary>
		/// <param name="left">Left-hand side operator.</param>
		/// <param name="right">Right-hand side operator.</param>
		/// <typeparam name="T">Type of the object to compare</typeparam>
		/// <returns>
		/// An in-memory representation of a delegate that describes a function which takes a value of type `T` and
		/// returns a `bool`, stored in an expression tree.
		/// </returns>
		public static Expression<Func<T, bool>> InvokeOr<T>(this Expression<Func<T, bool>> left,
			Expression<Func<T, bool>> right)
		{
			// Reference: http://www.albahari.com/nutshell/predicatebuilder.aspx
			// NOTE: This version works different to `PredicateBuilderLinq.Or` in that instead of replacing the
			// common parameters and constructing a new expression, it creates an expression that first evaluates the
			// left-hand side, and that passes the parameter from the lhs to the rhs expression, that is then being
			// evaluated. Lastly, the two results are `or` compared.
			
			// (t1 represents the lhs-parameter and t2 the rhs parameter; t1 is passed into t2)
			// lhs = t1 => false
			// rhs = t2 => t2.Tags.Contains(tag)
			// invokedExpr = () => Invoke(t2 => t2.Tags.Contains(tag), t1)
			var invokedExpr = Expression.Invoke(right, left.Parameters);
			//var invokedExpr = Expression.Invoke(right, left.Parameters.Cast<Expression>());

			// Invoke executes the rhs expression by passing it the parameter of the lhs expression
			// return = t1 => (false || Invoke(t2 => t2.Tags.Contains(tag), t1))
			return Expression.Lambda<Func<T, bool>>(Expression.OrElse(left.Body, invokedExpr), left.Parameters);
		}
	}
}