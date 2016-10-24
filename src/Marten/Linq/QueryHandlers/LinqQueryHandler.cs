using System;
using Marten.Linq.Model;
using Marten.Schema;
using Marten.Services.Includes;
using Npgsql;
using Remotion.Linq;

namespace Marten.Linq.QueryHandlers
{
    public class LinqQueryHandler<T> : ListQueryHandler<T>
    {
        private readonly LinqQuery<T> _query;

        public LinqQueryHandler(LinqQuery<T> query) : base(query.Selector)
        {
            _query = query;
        }

        [Obsolete("Just have LinqQuery injected in, or replace w/ LinqQuery")]
        public LinqQueryHandler(IDocumentSchema schema, QueryModel queryModel, IIncludeJoin[] includeJoins,
            QueryStatistics statistics) : this(new LinqQuery<T>(queryModel, schema, includeJoins, statistics))
        {
        }

        public override Type SourceType => _query.SourceType;

        public override void ConfigureCommand(NpgsqlCommand command)
        {
            _query.ConfigureCommand(command);
        }
    }
}