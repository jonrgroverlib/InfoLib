//--------------------------------------------------------------------------------------------------
// This file is part of the InfoLibCsLesserGpl version of Informationlib.
//
// InfoLib is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// InfoLib is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with InfoLib.  If not, see <http://www.gnu.org/licenses/>.
//--------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfoLib.SoftData // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InfoLib.Testing references
{
    public class SoftDouble : ISoft
    {
        public double Value { get { return _value; } set { _value = value; Provenance.Add(new SoftDouble(value)); } }  private double _value;
        public List<ISoft> Provenance { get; set; }

        public SoftDouble(double num)
        {
            _value = num;
            StartProvenance(num, 0);
        }
        private SoftDouble(double num, int infiniteLoop)
        {
            _value = num;
            if (infiniteLoop > 0)
                StartProvenance(num, infiniteLoop-1);
        }

        private void StartProvenance(double num, int infiniteLoop)
        {
            Provenance = new List<ISoft>();
            Provenance.Add(new SoftDouble(num, 0));
        }
    }
}
