﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Signum.Engine.Maps;
using Signum.Entities;
using Signum.Utilities;
using Signum.Utilities.Reflection;
using Signum.Engine.Properties;
using System.Data.SqlClient;
using Signum.Utilities.ExpressionTrees;

namespace Signum.Engine.Linq
{
    internal interface IProjectionRow
    {
        FieldReader Reader { get; }

        IRetriever Retriever { get; }

        IEnumerable<S> Lookup<K, S>(ProjectionToken token, K key);
    }

    internal class ProjectionRowEnumerator<T> : IProjectionRow, IEnumerator<T>
    {
        public FieldReader Reader { get; private set;}
        public IRetriever Retriever { get; private set; }

        public IProjectionRow Parent { get; private set; }

        SqlDataReader dataReader;

        T current;
        Func<IProjectionRow, T> projector; 
        Expression<Func<IProjectionRow, T>> projectorExpression;

        Dictionary<ProjectionToken, IEnumerable> lookups;

        internal ProjectionRowEnumerator(SqlDataReader dataReader, Expression<Func<IProjectionRow, T>> projectorExpression, Dictionary<ProjectionToken, IEnumerable> lookups, IRetriever retriever)
        {
            this.dataReader = dataReader;
            this.Reader = new FieldReader(dataReader);

            this.projectorExpression = ExpressionCompilableAsserter.Assert(projectorExpression);
            this.projector = projectorExpression.Compile();
            this.lookups = lookups;
            this.Row = -1;
            this.Retriever = retriever;
        }

        public T Current
        {
            get { return this.current; }
        }

        object IEnumerator.Current
        {
            get { return this.current; }
        }

        public int Row;
        public bool MoveNext()
        {
            if (dataReader.Read())
            {
                this.Row++;
                this.current = this.projector(this);
                return true;
            }
            return false;
        }

        public void Reset()
        {
        }

        public void Dispose()
        {
        }
 
        public IEnumerable<S> Lookup<K, S>(ProjectionToken token, K key)
        {
            Lookup<K, S> lookup = (Lookup<K, S>)lookups[token];

            if (!lookup.Contains(key))
                return Enumerable.Empty<S>();
            else
                return lookup[key];
        }
    }

    internal class ProjectionRowEnumerable<T> : IEnumerable<T>, IEnumerable
    {
        ProjectionRowEnumerator<T> enumerator;

        internal ProjectionRowEnumerable(ProjectionRowEnumerator<T> enumerator)
        {
            this.enumerator = enumerator;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Interlocked.Exchange(ref enumerator, null)
                .ThrowIfNullC("Cannot enumerate more than once");
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    internal class ExpressionCompilableAsserter : ExpressionVisitor
    {
        public override Expression Visit(Expression exp)
        {
            try
            {
                return base.Visit(exp);
            }
            catch (InvalidOperationException e)
            {
                if (e.Message.StartsWith("Unhandled Expression"))
                    throw new NotSupportedException("The expression can not be compiled:\r\n{0}".Formato(exp.NiceToString()));
                throw;
            }
        }

        internal static Expression<Func<IProjectionRow, T>> Assert<T>(Expression<Func<IProjectionRow, T>> projectorExpression)
        {
            return (Expression<Func<IProjectionRow, T>>)new ExpressionCompilableAsserter().Visit(projectorExpression);
        }
    }
}
