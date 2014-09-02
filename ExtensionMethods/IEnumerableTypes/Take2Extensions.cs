using System;
using System.Collections.Generic;

namespace Erwine.Leonard.T.ExtensionMethods.IEnumerableTypes
{
    /// <summary>
    /// Extension methods to support processing multiple collections
    /// </summary>
    /// <remarks>This provides functionality to process 2 enumerable collections without needing to enumerate any of the collections more than once.</remarks>
    public static class Take2Extensions
    {
        /// <summary>
        /// Filters a sequence of values based on a predicate, while providing access to the remaining Elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <param name="source">An <see cref="System.Collections.Generic.IEnumerable&lt;T&gt;"/> to filter.</param>
        /// <param name="predicate">A function to test each source element for a condition.</param>
        /// <returns>A <see cref="LoggingModule.ExtensionMethods.Take2Extensions.Take2ParseResult&lt;T&gt;"/> that contains elements from the input sequence that satisfy the condition.</returns>
        public static Take2ParseResult<T> Where2<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            return new Take2ParseResult<T>(source, predicate, false);
        }

        /// <summary>
        /// Filters a sequence of values based on a predicate, while providing access to the remaining Elements. Each element's index is used in the logic of the predicate function.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <param name="source">An <see cref="System.Collections.Generic.IEnumerable&lt;T&gt;"/> to filter.</param>
        /// <param name="predicate">A function to test each source element for a condition; the second parameter of the function represents the index of the source element.</param>
        /// <returns>A <see cref="LoggingModule.ExtensionMethods.Take2Extensions.Take2ParseResult&lt;T&gt;"/> that contains elements from the input sequence that satisfy the condition.</returns>
        public static Take2ParseResult<T> Where2<T>(this IEnumerable<T> source, Func<T, int, bool> predicate)
        {
            return new Take2ParseResult<T>(source, predicate, false);
        }

        /// <summary>
        /// Gets zero or more Elements from the beginning of the <paramref name="source"/> collection, while providing access to the remaining Elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence to parse.</param>
        /// <param name="count">Number of elements in first collection</param>
        /// <returns>A <see cref="LoggingModule.ExtensionMethods.Take2Extensions.Take2ParseResult&lt;T&gt;"/> that contains elements from the input sequence that satisfy the condition.</returns>
        /// <remarks>This has the same effect of the following Linq expressions:
        /// <code>IEnumerable&lt;<typeparamref name="T"/>$gt; collection1 = <paramref name="source"/>.Take(<paramref name="count"/>);
        /// IEnumerable&lt;<typeparamref name="T"/>$gt; collection2 = <paramref name="source"/>.Skip(<paramref name="count"/>);</code>
        /// <para>The difference is that <paramref name="source"/> is not enumerated twice to accomplish this effect.</para>
        /// <para>This extension method never returns null (although individual elements can be null if the source contains null elements):</para>
        /// <para>If <paramref name="source"/> is null, then an empty collection is returned.</para></remarks>
        public static Take2ParseResult<T> Take2<T>(this IEnumerable<T> source, int count)
        {
            int index = 0;
            return source.TakeWhile2<T>((T t) =>
            {
                if (index < count)
                {
                    index++;
                    return true;
                }

                return false;
            });
        }

        /// <summary>
        /// Conditionally gets zero or more Elements from the beginning of the <paramref name="source"/> collection, while providing access to the remaining Elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence to parse.</param>
        /// <param name="predicate">A function to test each element for a condition. Elements are included sequentially in the results until this returns false.</param>
        /// <returns>A <see cref="LoggingModule.ExtensionMethods.Take2Extensions.Take2ParseResult&lt;T&gt;"/> that contains elements from the input sequence that satisfy the condition.</returns>
        /// <remarks>This has the same effect of the following Linq expressions:
        /// <code>IEnumerable&lt;<typeparamref name="T"/>$gt; collection1 = <paramref name="source"/>.TakeWhile(<paramref name="predicate"/>);
        /// IEnumerable&lt;<typeparamref name="T"/>$gt; collection2 = <paramref name="source"/>.SkipWhile(<paramref name="predicate"/>);</code>
        /// <para>The difference is that <paramref name="source"/> is not enumerated twice to accomplish this effect.</para>
        /// <para>This extension method never returns null (although individual elements can be null if the source contains null elements):</para>
        /// <para>If <paramref name="source"/> or <paramref name="predicate"/> is null, then an empty collection is returned.</para></remarks>
        public static Take2ParseResult<T> TakeWhile2<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            return new Take2ParseResult<T>(source, predicate, true);
        }

        /// <summary>
        /// Conditionally gets zero or more Elements from the beginning of the <paramref name="source"/> collection, while providing access to the remaining Elements. Each element's index is used in the logic of the predicate function.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence to parse.</param>
        /// <param name="predicate">A function to test each element for a condition. Elements are included sequentially in the results until this returns false; the second parameter of the function represents the index of the source element.</param>
        /// <returns>A <see cref="LoggingModule.ExtensionMethods.Take2Extensions.Take2ParseResult&lt;T&gt;"/> that contains elements from the input sequence that satisfy the condition.</returns>
        /// <remarks>This has the same effect of the following Linq expressions:
        /// <code>IEnumerable&lt;<typeparamref name="T"/>$gt; collection1 = <paramref name="source"/>.TakeWhile(<paramref name="predicate"/>);
        /// IEnumerable&lt;<typeparamref name="T"/>$gt; collection2 = <paramref name="source"/>.SkipWhile(<paramref name="predicate"/>);</code>
        /// <para>The difference is that <paramref name="source"/> is not enumerated twice to accomplish this effect.</para>
        /// <para>This extension method never returns null (although individual elements can be null if the source contains null elements):</para>
        /// <para>If <paramref name="source"/> or <paramref name="predicate"/> is null, then an empty collection is returned.</para></remarks>
        public static Take2ParseResult<T> TakeWhile2<T>(this IEnumerable<T> source, Func<T, int, bool> predicate)
        {
            return new Take2ParseResult<T>(source, predicate, true);
        }

        /// <summary>
        /// Extracts zero or more elements from the <paramref name="source"/> collection, while providing access to the remaining Elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence to extract from.</param>
        /// <param name="index">Index at which to begin extraction</param>
        /// <param name="count">Number of elements to extract.</param>
        /// <returns>A <see cref="LoggingModule.ExtensionMethods.Take2Extensions.Take2ParseResult&lt;T&gt;"/> that contains elements from the input sequence that satisfy the condition.</returns>
        /// <remarks>This has the same effect of the following Linq expressions:
        /// <code>IEnumerable&lt;<typeparamref name="T"/> collection1 = <paramref name="source"/>.Skip(<paramref name="index"/>).Take(<paramref name="count"/>);
        /// IEnumerable&lt;<typeparamref name="T"/> collection2 = <paramref name="source"/>.Take(<paramref name="index"/>).Skip(<paramref name="count"/>);</code>
        /// <para>The difference is that <paramref name="source"/> is not enumerated twice to accomplish this effect.</para>
        /// <para>This extension method never returns null (although individual elements can be null if the source contains null elements):</para>
        /// <para>If <paramref name="source"/> is null, then an empty collection is returned.</para></remarks>
        public static Take2ParseResult<T> Extract<T>(this IEnumerable<T> source, int index, int count)
        {
            int c = 0;
            return source.ExtractWhile<T>(index, (T t) =>
            {
                if (c < count)
                {
                    c++;
                    return true;
                }

                return false;
            });
        }

        /// <summary>
        /// Extracts zero or more elements from the <paramref name="source"/> collection, while providing access to the remaining Elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence to extract from.</param>
        /// <param name="skipPredicate">A function to test each element for a condition. Elements are excluded sequentially from the results until this returns false.</param>
        /// <param name="count">Number of elements to extract.</param>
        /// <returns>A <see cref="LoggingModule.ExtensionMethods.Take2Extensions.Take2ParseResult&lt;T&gt;"/> that contains elements from the input sequence that satisfy the condition.</returns>
        /// <remarks>This has the same effect of the following Linq expressions:
        /// <code>IEnumerable&lt;<typeparamref name="T"/> collection1 = <paramref name="source"/>.SkipWhile(<paramref name="skipPredicate"/>).Take(<paramref name="count"/>);
        /// IEnumerable&lt;<typeparamref name="T"/> collection2 = <paramref name="source"/>.TakeWhile(<paramref name="skipPredicate"/>).Skip(<paramref name="count"/>);</code>
        /// <para>The difference is that <paramref name="source"/> is not enumerated twice to accomplish this effect.</para>
        /// <para>This extension method never returns null (although individual elements can be null if the source contains null elements):</para>
        /// <para>If <paramref name="source"/> is null, then an empty collection is returned.</para>
        /// <para>If <paramref name="skipPredicate"/> is null, then <paramref name="count"/> elements are returned from the beginning of <paramref name="source"/>.</para></remarks>
        public static Take2ParseResult<T> Extract<T>(this IEnumerable<T> source, Func<T, bool> skipPredicate, int count)
        {
            int c = 0;
            return new Take2ParseResult<T>(source, skipPredicate, (T t) =>
            {
                if (c < count)
                {
                    c++;
                    return true;
                }

                return false;
            });
        }

        /// <summary>
        /// Extracts zero or more elements from the <paramref name="source"/> collection, while providing access to the remaining Elements. Each element's index is used in the logic of the predicate function.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence to extract from.</param>
        /// <param name="skipPredicate">A function to test each element for a condition. Elements are excluded sequentially from the results until this returns false; the second parameter of the function represents the index of the source element.</param>
        /// <param name="count">Number of elements to extract.</param>
        /// <returns>A <see cref="LoggingModule.ExtensionMethods.Take2Extensions.Take2ParseResult&lt;T&gt;"/> that contains elements from the input sequence that satisfy the condition.</returns>
        /// <remarks>This has the same effect of the following Linq expressions:
        /// <code>IEnumerable&lt;<typeparamref name="T"/> collection1 = <paramref name="source"/>.SkipWhile(<paramref name="skipPredicate"/>).Take(<paramref name="count"/>);
        /// IEnumerable&lt;<typeparamref name="T"/> collection2 = <paramref name="source"/>.TakeWhile(<paramref name="skipPredicate"/>).Skip(<paramref name="count"/>);</code>
        /// <para>The difference is that <paramref name="source"/> is not enumerated twice to accomplish this effect.</para>
        /// <para>This extension method never returns null (although individual elements can be null if the source contains null elements):</para>
        /// <para>If <paramref name="source"/> is null, then an empty collection is returned.</para>
        /// <para>If <paramref name="skipPredicate"/> is null, then <paramref name="count"/> elements are returned from the beginning of <paramref name="source"/>.</para></remarks>
        public static Take2ParseResult<T> Extract<T>(this IEnumerable<T> source, Func<T, int, bool> skipPredicate, int count)
        {
            int c = 0;
            return new Take2ParseResult<T>(source, skipPredicate, (T t, int index) =>
            {
                if (c < count)
                {
                    c++;
                    return true;
                }

                return false;
            });
        }

        /// <summary>
        /// Conditonally extracts zero or more elements from the <paramref name="source"/> collection, while providing access to the remaining Elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence to extract from.</param>
        /// <param name="index">Index at which to begin extraction</param>
        /// <param name="takePredicate">A function to test each element for a condition. Elements which occur sequentally after the <paramref name="index"/>ed position are included in the results until this returns false.</param>
        /// <returns>A <see cref="LoggingModule.ExtensionMethods.Take2Extensions.Take2ParseResult&lt;T&gt;"/> that contains elements from the input sequence that satisfy the condition.</returns>
        /// <remarks>This has the same effect of the following Linq expressions:
        /// <code>IEnumerable&lt;<typeparamref name="T"/> collection1 = <paramref name="source"/>.Skip(<paramref name="index"/>).TakeWhile(<paramref name="takePredicate"/>);
        /// IEnumerable&lt;<typeparamref name="T"/> collection2 = <paramref name="source"/>.Take(<paramref name="index"/>).SkipWhile(<paramref name="takePredicate"/>);</code>
        /// <para>The difference is that <paramref name="source"/> is not enumerated twice to accomplish this effect.</para>
        /// <para>This extension method never returns null (although individual elements can be null if the source contains null elements):</para>
        /// <para>If <paramref name="source"/> or <paramref name="takePredicate"/> is null, then an empty collection is returned.</para></remarks>
        public static Take2ParseResult<T> ExtractWhile<T>(this IEnumerable<T> collection, int index, Func<T, bool> takePredicate)
        {
            int i = 0;
            return new Take2ParseResult<T>(collection, (T t) =>
            {
                if (i < index)
                {
                    i++;
                    return true;
                }

                return false;
            }, takePredicate);
        }

        /// <summary>
        /// Conditonally extracts zero or more elements from the <paramref name="source"/> collection, while providing access to the remaining Elements. Each element's index is used in the logic of the predicate function.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence to extract from.</param>
        /// <param name="index">Index at which to begin extraction</param>
        /// <param name="takePredicate">A function to test each element for a condition. Elements which occur sequentally after the <paramref name="index"/>ed position are included in the results until this returns false; the second parameter of the function represents the index of the source element.</param>
        /// <returns>A <see cref="LoggingModule.ExtensionMethods.Take2Extensions.Take2ParseResult&lt;T&gt;"/> that contains elements from the input sequence that satisfy the condition.</returns>
        /// <remarks>This has the same effect of the following Linq expressions:
        /// <code>IEnumerable&lt;<typeparamref name="T"/> collection1 = <paramref name="source"/>.Skip(<paramref name="index"/>).TakeWhile(<paramref name="takePredicate"/>);
        /// IEnumerable&lt;<typeparamref name="T"/> collection2 = <paramref name="source"/>.Take(<paramref name="index"/>).SkipWhile(<paramref name="takePredicate"/>);</code>
        /// <para>The difference is that <paramref name="source"/> is not enumerated twice to accomplish this effect.</para>
        /// <para>This extension method never returns null (although individual elements can be null if the source contains null elements):</para>
        /// <para>If <paramref name="source"/> or <paramref name="takePredicate"/> is null, then an empty collection is returned.</para></remarks>
        public static Take2ParseResult<T> ExtractWhile<T>(this IEnumerable<T> collection, int index, Func<T, int, bool> takePredicate)
        {
            int i = 0;
            return new Take2ParseResult<T>(collection, (T t, int ti) =>
            {
                if (i < index)
                {
                    i++;
                    return true;
                }

                return false;
            }, takePredicate);
        }

        /// <summary>
        /// Conditonally extracts zero or more elements from the <paramref name="source"/> collection, while providing access to the remaining Elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence to extract from.</param>
        /// <param name="skipPredicate">A function to test each element for a condition. Elements are excluded sequentially from the results until this returns false.</param>
        /// <param name="takePredicate">A function to test each element for a condition. Elements are included sequentially, following the skipped items, in the results until this returns false.</param>
        /// <returns>A <see cref="LoggingModule.ExtensionMethods.Take2Extensions.Take2ParseResult&lt;T&gt;"/> that contains elements from the input sequence that satisfy the condition.</returns>
        /// <remarks>This has the same effect of the following Linq expressions:
        /// <code>IEnumerable&lt;<typeparamref name="T"/> collection1 = <paramref name="source"/>.SkipWhile(<paramref name="skipPredicate"/>).TakeWhile(<paramref name="takePredicate"/>);
        /// IEnumerable&lt;<typeparamref name="T"/> collection2 = <paramref name="source"/>.TakeWhile(<paramref name="skipPredicate"/>).SkipWhile(<paramref name="takePredicate"/>);</code>
        /// <para>The difference is that <paramref name="source"/> is not enumerated twice to accomplish this effect.</para>
        /// <para>This extension method never returns null (although individual elements can be null if the source contains null elements):</para>
        /// <para>If <paramref name="source"/> or <paramref name="takePredicate"/> is null, then an empty collection is returned.</para>
        /// <para>If <paramref name="skipPredicate"/> is null, it is ignored.</para></remarks>
        public static Take2ParseResult<T> ExtractWhile<T>(this IEnumerable<T> source, Func<T, bool> skipPredicate, Func<T, bool> takePredicate)
        {
            return new Take2ParseResult<T>(source, skipPredicate, takePredicate);
        }

        /// <summary>
        /// Conditonally extracts zero or more elements from the <paramref name="source"/> collection, while providing access to the remaining Elements. Each element's index is used in the logic of the predicate function.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence to extract from.</param>
        /// <param name="skipPredicate">A function to test each element for a condition. Elements are excluded sequentially from the results until this returns false; the second parameter of the function represents the index of the source element.</param>
        /// <param name="takePredicate">A function to test each element for a condition. Elements are included sequentially, following the skipped items, in the results until this returns false; the second parameter of the function represents the index of the source element.</param>
        /// <returns>A <see cref="LoggingModule.ExtensionMethods.Take2Extensions.Take2ParseResult&lt;T&gt;"/> that contains elements from the input sequence that satisfy the condition.</returns>
        /// <remarks>This has the same effect of the following Linq expressions:
        /// <code>IEnumerable&lt;<typeparamref name="T"/> collection1 = <paramref name="source"/>.SkipWhile(<paramref name="skipPredicate"/>).TakeWhile(<paramref name="takePredicate"/>);
        /// IEnumerable&lt;<typeparamref name="T"/> collection2 = <paramref name="source"/>.TakeWhile(<paramref name="skipPredicate"/>).SkipWhile(<paramref name="takePredicate"/>);</code>
        /// <para>The difference is that <paramref name="source"/> is not enumerated twice to accomplish this effect.</para>
        /// <para>This extension method never returns null (although individual elements can be null if the source contains null elements):</para>
        /// <para>If <paramref name="source"/> or <paramref name="takePredicate"/> is null, then an empty collection is returned.</para>
        /// <para>If <paramref name="skipPredicate"/> is null, it is ignored.</para></remarks>
        public static Take2ParseResult<T> ExtractWhile<T>(this IEnumerable<T> source, Func<T, int, bool> skipPredicate, Func<T, int, bool> takePredicate)
        {
            return new Take2ParseResult<T>(source, skipPredicate, takePredicate);
        }

        /// <summary>
        /// Inserts elements from <paramref name="toInsert"/> into the <paramref name="source"/> collection. Each element's index is used in the logic of the predicate function.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence to insert <paramref name="toInsert"/> elements into.</param>
        /// <param name="index">Index at which to insert <paramref name="toInsert"/> elements</param>
        /// <param name="toInsert">Elements to insert.</param>
        /// <returns>An collection with <paramref name="toInsert"/> inserted into <paramref name="source"/>.</returns>
        /// <remarks>This has the same effect of the following Linq expressions:
        /// <code>IEnumerable&lt;<typeparamref name="T"/> result = <paramref name="source"/>.Take(<paramref name="index"/>).Concat(<paramref name="toInsert"/>)
        /// .Concat(<paramref name="source"/>.Skip(<paramref name="index"/>));</code>
        /// <para>The difference is that <paramref name="source"/> is not enumerated twice to accomplish this effect.</para>
        /// <para>This extension method never returns null (although individual elements can be null if the source contains null elements):</para>
        /// <para>If both <paramref name="source"/> and <paramref name="toInsert"/> are null, then an empty collection is returned.</para>
        /// <para>If only <paramref name="source"/> is null, then <paramref name="toInsert"/> is returned.</para>
        /// <para>Likewise, If only <paramref name="toInsert"/> is null, then <paramref name="source"/> is returned.</para></remarks>
        public static IEnumerable<T> Insert<T>(this IEnumerable<T> source, int index, IEnumerable<T> toInsert)
        {
            return new Take2InsertResult<T>(source.Take2<T>(index), toInsert);
        }

        /// <summary>
        /// Inserts elements from <paramref name="toInsert"/> into the <paramref name="source"/> collection.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence to insert <paramref name="toInsert"/> elements into.</param>
        /// <param name="predicate">A function to test each element for a condition. Elements are excluded sequentially from the results until this returns false.</param>
        /// <param name="toInsert">Elements to insert.</param>
        /// <returns>An collection with <paramref name="toInsert"/> inserted into <paramref name="source"/>.</returns>
        /// <remarks>This has the same effect of the following Linq expressions:
        /// <code>IEnumerable&lt;<typeparamref name="T"/> result = <paramref name="source"/>.TakeWhile(<paramref name="predicate"/>).Concat(<paramref name="toInsert"/>)
        /// .Concat(<paramref name="source"/>.SkipWhile(<paramref name="predicate"/>));</code>
        /// <para>The difference is that <paramref name="source"/> is not enumerated twice to accomplish this effect.</para>
        /// <para>This extension method never returns null (although individual elements can be null if the source contains null elements):</para>
        /// <para>If both <paramref name="source"/> and <paramref name="toInsert"/> are null, then an empty collection is returned.</para>
        /// <para>If only <paramref name="source"/> is null, then <paramref name="toInsert"/> is returned.</para>
        /// <para>Likewise, If only <paramref name="toInsert"/> is null, then <paramref name="source"/> is returned.</para>
        /// <para>If <paramref name="predicate"/> is null, then <paramref name="toInsert"/> will be inserted at the beginning.</para></remarks>
        public static Take2InsertResult<T> InsertAfter<T>(this IEnumerable<T> source, Func<T, bool> predicate, IEnumerable<T> toInsert)
        {
            return new Take2InsertResult<T>(source.TakeWhile2<T>(predicate), toInsert);
        }

        /// <summary>
        /// Inserts elements from <paramref name="toInsert"/> into the <paramref name="source"/> collection. Each element's index is used in the logic of the predicate function.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence to insert <paramref name="toInsert"/> elements into.</param>
        /// <param name="predicate">A function to test each element for a condition. Elements are excluded sequentially from the results until this returns false; the second parameter of the function represents the index of the source element.</param>
        /// <param name="toInsert">Elements to insert.</param>
        /// <returns>An collection with <paramref name="toInsert"/> inserted into <paramref name="source"/>.</returns>
        /// <remarks>This has the same effect of the following Linq expressions:
        /// <code>IEnumerable&lt;<typeparamref name="T"/> result = <paramref name="source"/>.TakeWhile(<paramref name="predicate"/>).Concat(<paramref name="toInsert"/>)
        /// .Concat(<paramref name="source"/>.SkipWhile(<paramref name="predicate"/>));</code>
        /// <para>The difference is that <paramref name="source"/> is not enumerated twice to accomplish this effect.</para>
        /// <para>This extension method never returns null (although individual elements can be null if the source contains null elements):</para>
        /// <para>If both <paramref name="source"/> and <paramref name="toInsert"/> are null, then an empty collection is returned.</para>
        /// <para>If only <paramref name="source"/> is null, then <paramref name="toInsert"/> is returned.</para>
        /// <para>Likewise, If only <paramref name="toInsert"/> is null, then <paramref name="source"/> is returned.</para>
        /// <para>If <paramref name="predicate"/> is null, then <paramref name="toInsert"/> will be inserted at the beginning.</para></remarks>
        public static Take2InsertResult<T> InsertAfter<T>(this IEnumerable<T> source, Func<T, int, bool> predicate, IEnumerable<T> toInsert)
        {
            return new Take2InsertResult<T>(source.TakeWhile2<T>(predicate), toInsert);
        }

        /// <summary>
        /// Combines 2 collections into parent/child relationships
        /// </summary>
        /// <typeparam name="TParent">Type of parent object.</typeparam>
        /// <typeparam name="TChild">TYpe of child object</typeparam>
        /// <param name="parents">Source collection of <typeparamref name="TParent"/> objects.</param>
        /// <param name="children">Source colleciton of <typeparamref name="TChild"/> objects</param>
        /// <param name="predicate">A function to test each <typeparamref name="TChild"/> object against each <typeparamref name="TParent"/> object to determine if that child belongs to the parent</param>
        /// <returns>A collection of <see cref="Take2RelationResult&lt;TParent, TChild&gt;"/> objects representing each parent with its associated child objects. The result also contains a property that contains all orphan <typeparamref name="TChild"/> objects.</returns>
        /// <remarks>With this override, each <typeparamref name="TChild"/> object will only be associated with one <typeparamref name="TParent"/> object</remarks>
        public static Take2RelationResult<TParent, TChild> OneToMany<TParent, TChild>(this IEnumerable<TParent> parents, IEnumerable<TChild> children, Func<TParent, TChild, bool> predicate)
        {
            return parents.OneToMany<TParent, TChild>(children, predicate, false);
        }

        /// <summary>
        /// Combines 2 collections into parent/child relationships
        /// </summary>
        /// <typeparam name="TParent">Type of parent object.</typeparam>
        /// <typeparam name="TChild">TYpe of child object</typeparam>
        /// <param name="parents">Source collection of <typeparamref name="TParent"/> objects.</param>
        /// <param name="children">Source colleciton of <typeparamref name="TChild"/> objects</param>
        /// <param name="predicate">A function to test each <typeparamref name="TChild"/> object against each <typeparamref name="TParent"/> object to determine if that child belongs to the parent</param>
        /// <param name="isExclusive">Set to true if each <typeparamref name="TChild"/> object can be associated with only one <typeparamref name="TParent"/> object; false if a single <typeparamref name="TChild"/> object can belong to multiple <typeparamref name="TParent"/> objects.</param>
        /// <returns>A collection of <see cref="Take2RelationResult&lt;TParent, TChild&gt;"/> objects representing each parent with its associated child objects. The result also contains a property that contains all orphan <typeparamref name="TChild"/> objects.</returns>
        public static Take2RelationResult<TParent, TChild> OneToMany<TParent, TChild>(this IEnumerable<TParent> parents, IEnumerable<TChild> children, Func<TParent, TChild, bool> predicate, bool isExclusive)
        {
            return new Take2RelationResult<TParent, TChild>(parents, children, predicate, isExclusive);
        }

        /// <summary>
        /// Convert to an IEnumerable collection where the source colleciton will only be enumerated once.
        /// </summary>
        /// <typeparam name="T">Type of source element to be enumerated</typeparam>
        /// <param name="source">Collection of <typeparamref name="T"/> elements to enumerate</param>
        /// <returns>A <see cref="LoggingModule.ExtensionMethods.Take2Extensions.NonReiteratingEnumerable&ltT&gt;"/> collection.</returns>
        public static NonReiteratingEnumerable<T> AsNonReiterating<T>(this IEnumerable<T> source)
        {
            return new NonReiteratingEnumerable<T>(source);
        }
    }
}
