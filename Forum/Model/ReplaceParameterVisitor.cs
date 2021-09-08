using System.Linq;
using System.Linq.Expressions;

namespace Forum.Model
{
	/// <summary>
	/// `ReplaceParameterVisitor` provides a method to replace a parameter in one expression
	/// with a parameter from a different expression.
	/// For reference see: https://www.c-sharpcorner.com/UploadFile/04fe4a/predicate-combinators-in-linq/
	/// </summary>
	/// <typeparam name="TResult">Type of the result the expression has to return.</typeparam>
	public class ReplaceParameterVisitor<TResult> : ExpressionVisitor
	{
		/* NOTE: The `ExpressionVisitor` is a class designed to be inherited to create more
		 * specialized classes whose functionality requires traversing, examining or copying
		 * an expression tree.
		 */
		
		/// <summary>
		/// Parameter to replace.
		/// An expression tree node, inherits from `Expression`.
		/// </summary>
		private readonly ParameterExpression _parameter;
		
		/// <summary>
		/// Replacement-Parameter.
		/// </summary>
		private readonly Expression _replacement;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="parameter">Parameter to replace.</param>
		/// <param name="replacement">Replacement-Parameter.</param>
		public ReplaceParameterVisitor(ParameterExpression parameter, Expression replacement)
		{
			_parameter = parameter;     // t2
			_replacement = replacement; // t1
		}
		
		/// <summary>
		/// Replace the `_parameter` in the expression with the `_replacement`.
		/// </summary>
		/// <param name="node">Expression to do the replacement in.</param>
		/// <typeparam name="T">Type of the expression.</typeparam>
		/// <returns>The new expression, with the parameters replaced.</returns>
		public Expression<TResult> Visit<T>(Expression<T> node)
		{
			// Get the other parameters, that we want to keep
			// NOTE: `t2 => t2.Tags.Contains(tag)` does not have any other parameters, therefore `parameters` will
			// remain empty.
			var parameters = node.Parameters.Where(p => p != _parameter);
			
			// Create a new expression using the `parameters` we want to keep and a newly constructed 
			// body in where we replaced all occurrences of t2 with t1.
			// NOTE: `base.Visit` below invokes the more specified `ReplaceParameterVisitor.VisitParameter`.
			// `VisitParameter` walks through the expression and replaces all occurrences of t2 with t1.
			// Before `VisitParameter`: node.Body = t2.Tags.Contains(tag)
			// After `VisitParameter`:  node.Body = t1.Tags.Contains(tag)
			// return = () => t1.Tags.Contains(tags)
			// If `node` would have more parameters, they would still be there:
			// return = (p1, p2) => t1.Tags.Contains(tags)
			return Expression.Lambda<TResult>(Visit(node.Body), parameters);
		}

		/// <summary>
		/// Replace all occurrences of `_parameter` with `_replacement`.
		/// </summary>
		/// <param name="node">The current parameter.</param>
		/// <returns>
		/// Returns `_replacement` if `node` is of `_parameter`, else the original `node`.
		/// </returns>
		protected override Expression VisitParameter(ParameterExpression node)
		{
			// If `node` is `_parameter` we want to replace it, else we want to keep it.
			return node == _parameter ? _replacement : base.VisitParameter(node);
		}
	}
}