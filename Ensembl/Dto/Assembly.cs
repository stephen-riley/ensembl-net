using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;

namespace Ensembl.Dto
{
    public class Assembly
    {
        public int AssemblySeqRegionId { get; private set; }

        public int ComponentSeqRegionId { get; private set; }

        public int AssemblyStart { get; private set; }

        public int AssemblyEnd { get; private set; }

        public int ComponentStart { get; private set; }

        public int ComponentEnd { get; private set; }

        private int orientation;
        public int Orientation
        {
            get => orientation;
            private set
            {
                if (value == 1 || value == -1)
                {
                    orientation = value;
                }
                else
                {
                    throw new ArgumentException("must be 1 or -1", nameof(Orientation));
                }
            }
        }

        public string ReverseComplement(ReadOnlyMemory<char> seq)
        {
            var compl = new Dictionary<char, char>
            {
                ['A'] = 'T',
                ['T'] = 'A',
                ['G'] = 'C',
                ['C'] = 'G',
                ['N'] = 'N'
            };

            var sb = new StringBuilder(seq.Length);
            for (var i = seq.Length - 1; i >= 0; i--)
            {
                sb.Append(compl[seq.Span[i]]);
            }

            return sb.ToString();
        }

        public int AsmToCmp(int pos)
        {
            if (pos < AssemblyStart || pos > AssemblyEnd)
            {
                throw new ArgumentException($"{nameof(pos)}({pos}) must be within AssemblyStart({AssemblyStart}) and AssemblyEnd({AssemblyEnd})", nameof(pos));
            }

            var ds = pos - AssemblyStart;

            return Orientation == 1
                ? ComponentStart + ds
                : ComponentEnd - ds;
        }

        public bool PositionWithinAssembly(int pos)
        {
            return AssemblyStart <= pos && pos <= AssemblyEnd;
        }

        public Assembly(int asm_seq_region_id, int asm_start, int asm_end, int cmp_seq_region_id, int cmp_start, int cmp_end, int ori)
        {
            AssemblySeqRegionId = asm_seq_region_id;
            AssemblyStart = asm_start;
            AssemblyEnd = asm_end;
            ComponentSeqRegionId = cmp_seq_region_id;
            ComponentStart = cmp_start;
            ComponentEnd = cmp_end;
            Orientation = ori;
        }

        internal static IEnumerable<Assembly> GetAssemblies(IDbConnection conn, int asmSeqRegionId)
        {
            var sql = @"select * from assembly where asm_seq_region_id = @Id";

            return conn.Query<Assembly>(sql, new { Id = asmSeqRegionId });
        }
    }
}