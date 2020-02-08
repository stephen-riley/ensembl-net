using System;
using System.Collections.Generic;
using System.Linq;
using Ensembl.Dto;
using Ensembl.Exceptions;

namespace Ensembl
{
    public class Slice
    {
        public string SpeciesDbName { get; private set; }
        private Chromosome? chromosome;

        public string ChromosomeName { get; private set; }

        public string this[int position]
        {
            get
            {
                return GetSequenceString(position, position);
            }
        }

        public string this[Range range]
        {
            get
            {
                return GetSequenceString(range.Start.Value, range.End.Value);
            }
        }

        public int Length => SpeciesCache.ByName(SpeciesDbName).GetChromosome(ChromosomeName).Length;

        public Slice(string species, string chromosomeName)
        {
            var speciesDbName = SpeciesCache.GetDbNameForCommonName(species);
            if (speciesDbName == null)
            {
                throw new EnsemblException($"species name '{species}' not found");
            }

            SpeciesDbName = speciesDbName;
            ChromosomeName = chromosomeName;
        }

        public string GetSequenceString(int start = 1, int end = Int32.MaxValue)
        {
            return String.Join("", GetSequenceStrings(start, end));
        }

        public IEnumerable<string> GetSequenceStrings(int start = 1, int end = Int32.MaxValue)
        {
            var seqs = new List<ReadOnlyMemory<char>>();

            chromosome = SpeciesCache.ByName(SpeciesDbName).GetChromosome(ChromosomeName);

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
                        var seq = SpeciesCache.ByName(SpeciesDbName).GetDnaSequence(asm.ComponentSeqRegionId, asm.AsmToCmp(adjStart), asm.AsmToCmp(adjEnd));
                        seqs.Add(seq);
                    }
                    else
                    {
                        var seq = SpeciesCache.ByName(SpeciesDbName).GetDnaSequence(asm.ComponentSeqRegionId, asm.AsmToCmp(adjEnd), asm.AsmToCmp(adjStart));
                        var revComp = asm.ReverseComplement(seq).AsMemory();
                        seqs.Add(revComp);
                    }
                }

                if (asm.AssemblyEnd >= end)
                {
                    break;
                }
            }

            return seqs.Select(seq => seq.ToString());
        }

        public static IEnumerable<string> GetSpeciesDbNames()
        {
            return SpeciesCache.GetInstalledEnsemblDatabases();
        }

        public static IEnumerable<string> GetSpeciesChromosomeList(string species)
        {
            var speciesDbName = SpeciesCache.GetDbNameForCommonName(species);
            if (speciesDbName == null)
            {
                throw new EnsemblException($"species name '{species}' not found");
            }

            return SpeciesCache.ByName(speciesDbName).GetChromosomeList();
        }
    }
}