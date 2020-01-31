using System;
using System.Collections.Generic;
using System.Linq;
using Ensembl.Dto;

namespace Ensembl
{
    public class Slice
    {
        public string Species { get; private set; }
        private Chromosome? chromosome;

        public string ChromosomeName { get; private set; }

        public string this[long position]
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string this[Range range]
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Slice(string species, string chromosomeName)
        {
            Species = species;
            ChromosomeName = chromosomeName;
        }

        public string GetSequenceString(int start = 1, int end = Int32.MaxValue)
        {
            return String.Join("", GetSequenceStrings(start, end));
        }

        public IEnumerable<string> GetSequenceStrings(int start = 1, int end = Int32.MaxValue)
        {
            // TODO: figure out proper species handling

            var seqs = new List<ReadOnlyMemory<char>>();

            chromosome = Cache.GetChromosome(ChromosomeName);

            foreach (var asm in chromosome.Assemblies)
            {
                if (asm.AssemblyEnd < start)
                {
                    continue;
                }

                var adjStart = asm.PositionWithinAssembly(start)
                    ? start
                    : asm.AssemblyStart;

                var adjEnd = asm.AssemblyEnd >= end
                    ? end
                    : asm.AssemblyEnd;

                if (asm is NAssembly)
                {
                    var seq = new String('N', adjEnd - adjStart + 1);
                    seqs.Add(seq.AsMemory());
                }
                else
                {
                    if (asm.Orientation == 1)
                    {
                        var seq = Cache.GetDnaSequence(asm.ComponentSeqRegionId, asm.AsmToCmp(adjStart), asm.AsmToCmp(adjEnd));
                        seqs.Add(seq);
                    }
                    else
                    {
                        var seq = Cache.GetDnaSequence(asm.ComponentSeqRegionId, asm.AsmToCmp(adjEnd), asm.AsmToCmp(adjStart));
                        var revComp = asm.ReverseComplement(seq).AsMemory();
                        seqs.Add(revComp);
                    }
                }

                if (asm.AssemblyEnd > end)
                {
                    break;
                }
            }

            return seqs.Select(seq => seq.ToString());
        }
    }
}