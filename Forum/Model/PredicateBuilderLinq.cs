using System;
using System.Linq.Expressions;

namespace Forum.Model
{
	/// <summary>
	/// Helper to combine predicates in LINQ-to-Entities (SQL).
	/// To be used in `IQueryable` collections.
	/// For reference see: https://www.c-sharpcorner.com/UploadFile/04fe4a/predicate-combinators-in-linq/
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

		// v-- TODO: ? --v
		
		/// <summary>
		/// Get the new (right-hand side) predicate containing only its unique parameters. (Remove the common
		/// parameters).
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <returns></returns>
		private static Expression<Func<TResult>> WithParametersOf<T, TResult>(this Expression<Func<T, TResult>> left,
			Expression<Func<T, TResult>> right)
		{
			return new ReplaceParameterVisitor<Func<TResult>>(left.Parameters[0], right.Parameters[0]).Visit(left);
		}

		/// <summary>
		/// Predicate `and` comparison.
		/// </summary>
		/// <param name="left">Left-hand side operator.</param>
		/// <param name="right">Right-hand side operator.</param>
		/// <typeparam name="T">Type of the object to compare.</typeparam>
		/// <returns>
		/// An in-memory representation of a delegate that describes a function which takes a value `T` and returns a
		/// `bool`, stored in an expression tree.
		/// </returns>
		public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left,
			Expression<Func<T, bool>> right)
		{
			// NOTE: `WithParametersOf` removes the common parameters from the "to be combined"-expression. This way
			// the common parameters only appear _once_ instead of multiple times.
			// (We want to add two expressions together, both take a parameter "t". In the combined expression,
			// parameter t can only appear once.)
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
		/// An in-memory representation of a delegate that describes a function which takes a value `T` and returns a
		/// `bool`, stored in an expression tree.
		/// </returns>
		public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> left,
			Expression<Func<T, bool>> right)
		{
			// NOTE: `WithParametersOf` removes the common parameters from the "to be combined"-expression. This way
			// the common parameters only appear _once_ instead of multiple times.
			// (We want to add two expressions together, both take a parameter "t". In the combined expression,
			// parameter t can only appear once.)
			// Example:
			// Without stripping common parameters, the parameter is now defined twice:
			// t => false + t => t.Tags.Contains(value(Forum.Model.SearchQuery_1).tag) results in:
			// t => false OrElse t => t.Tags.Contains(value(Forum.Model.SearchQuery_1).tag)
			//
			// Removing the common parameter first:
			// t => false + t.Tags.Contains(value(Forum.Model.SearchQuery_1).tag) results in:
			// = t => false OrElse t.Tags.Contains(value(Forum.Model.SearchQuery_1).tag)
			return Expression.Lambda<Func<T, bool>>(Expression.OrElse(left.Body, right.WithParametersOf(left).Body),
				left.Parameters);
		}
	}
}