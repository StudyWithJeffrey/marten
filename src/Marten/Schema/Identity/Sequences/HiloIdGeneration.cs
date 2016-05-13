using System;
using System.Collections.Generic;
using System.Reflection;
using Marten.Util;

namespace Marten.Schema.Identity.Sequences
{
    public class HiloIdGeneration : StorageArgument, IIdGeneration
    {
        private readonly HiloSettings _hiloSettings;
        public Type DocumentType { get; private set; }

        public HiloIdGeneration(Type documentType, HiloSettings hiloSettings) : base("sequence", typeof(ISequence))
        {
            _hiloSettings = hiloSettings;
            DocumentType = documentType;
        }

        public int Increment => _hiloSettings.Increment;
        public int MaxLo => _hiloSettings.MaxLo;

        public override object GetValue(IDocumentSchema schema)
        {
            return schema.Sequences.Hilo(DocumentType, _hiloSettings);
        }

        public IEnumerable<StorageArgument> ToArguments()
        {
            return new StorageArgument[] {this};
        }

        public string AssignmentBodyCode(MemberInfo idMember)
        {
            var member = idMember.GetMemberType() == typeof (int) ? "NextInt" : "NextLong";

            return $@"
BLOCK:if (document.{idMember.Name} == 0) 
document.{idMember.Name} = _sequence.{member}();
assigned = true;
END
BLOCK:else
assigned = false;
END
";
        }

        public IEnumerable<Type> KeyTypes { get; } = new Type[] {typeof(int), typeof(long)};


        public IIdGenerator<T> Build<T>(IDocumentSchema schema)
        {
            var sequence = schema.Sequences.Hilo(DocumentType, _hiloSettings);

            if (typeof(T) == typeof(int))
            {
                return (IIdGenerator<T>) new IntHiloGenerator(sequence);
            }

            return (IIdGenerator<T>) new LongHiloGenerator(sequence);
        }


    }
}