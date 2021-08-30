using System;

namespace Forum.Model
{
	/// <summary>
	/// Helper to combine predicates.
	/// To be used for `IEnumerable` collections.
	/// Don't use in LINQ for Entities, this can causes the program
	/// to pull entire databases into memory for processing.
	/// </summary>
	public static class PredicatesBuilder
	{
		/// <summary>
		/// Default to `true`.
		/// </summary>
		/// <typeparam name="T">Type of object to compare.</typeparam>
		/// <returns>A delegate that describes a function which takes a value `T` and returns a `bool`.</returns>
		public static Func<T, bool> True<T>()
		{
			// ReSharper disable once UnusedParameter.Local
			return t => true; // returns a function that takes a parameter of type `T` and always returns `true`.
		}

		/// <summary>
		/// Default to `false`.
		/// </summary>
		/// <typeparam name="T">Type of object to compare.</typeparam>
		/// <returns>A delegate that describes a function which takes a value `T` and returns a `bool`.</returns>
		public static Func<T, bool> False<T>()
		{
			// ReSharper disable once UnusedParameter.Local
			return t => false; // returns a function that takes a parameter of type `T` and always returns `false`.
		}

		/// <summary>
		/// Predicate `or` comparison.
		/// </summary>
		/// <param name="left">Left-hand side operator.</param>
		/// <param name="right">Right-hand side operator.</param>
		/// <typeparam name="T">Type of the object to compare.</typeparam>
		/// <returns>A delegate that describes a function which takes a value `T` and returns a `bool`.</returns>
		public static Func<T, bool> Or<T>(this Func<T, bool> left, Func<T, bool> right)
		{
			return t => left(t) || right(t); // returns `true` if the delegate on the left _or_ the delegate on the right evaluate to `true`.
		}

		/// <summary>
		/// Predicate `and` comparison.
		/// </summary>
		/// <param name="left">Left-hand side operator.</param>
		/// <param name="right">Right-hand side operator.</param>
		/// <typeparam name="T">Type of the object to compare.</typeparam>
		/// <returns>A delegate that describes a function which takes a value `T` and returns a `bool`.</returns>
		public static Func<T, bool> And<T>(this Func<T, bool> left, Func<T, bool> right)
		{
			return t => left(t) && right(t); // returns `true` if the delegate on the left _and_ right both evaluate to `true`.
		}

		/// <summary>
		/// Predicate `not` operator. Works like `if (!predicate)`.
		/// </summary>
		/// <param name="predicate">Predicate to invert.</param>
		/// <typeparam name="T">Type of the object to compare.</typeparam>
		/// <returns>A delegate that describes a function which takes a value `T` and returns a `bool`.</returns>
		public static Func<T, bool> Not<T>(this Func<T, bool> predicate)
		{
			return t => !predicate(t); // returns `true` if the delegate evaluates to `false`.
		}
	}
}