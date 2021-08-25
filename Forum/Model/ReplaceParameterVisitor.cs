using System.Linq;
using System.Linq.Expressions;

namespace Forum.Model
{
	/// <summary>
	/// TODO: ?
	/// For reference see: https://www.c-sharpcorner.com/UploadFile/04fe4a/predicate-combinators-in-linq/
	/// </summary>
	/// <typeparam name="TResult">TODO: ?</typeparam>
	public class ReplaceParameterVisitor<TResult> : ExpressionVisitor
	{
		/* NOTE: The `ExpressionVisitor` is a class designed to be inherited to create more
		 * specialized classes whose functionality requires traversing, examining or copying
		 * an expression tree.
		 */
		
		/// <summary>
		/// An expression tree node, inherits from `Expression`.
		/// </summary>
		private readonly ParameterExpression _parameter;
		
		/// <summary>
		/// An expression tree node.
		/// </summary>
		private readonly Expression _replacement;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="parameter">TODO: ?</param>
		/// <param name="replacement">TODO: ?</param>
		public ReplaceParameterVisitor(ParameterExpression parameter, Expression replacement)
		{
			_parameter = parameter;
			_replacement = replacement;
		}
		
		/// <summary>
		/// Remove the common parameters from the predicate.
		/// </summary>
		/// <param name="node">Predicate to remove the parameters from.</param>
		/// <typeparam name="T">Type of the predicate expression.</typeparam>
		/// <returns>The predicate stripped from its common parameters.</returns>
		public Expression<TResult> Visit<T>(Expression<T> node)
		{
			// Get `node`s unique parameters.
			var parameters = node.Parameters.Where(p => p != _parameter);
			
			// Create a new expression using only `node`s unique parameters.
			return Expression.Lambda<TResult>(Visit(node.Body), parameters);
		}

		protected override Expression VisitParameter(ParameterExpression node)
		{
			return node == _parameter ? _replacement : base.VisitParameter(node);
		}
	}
}