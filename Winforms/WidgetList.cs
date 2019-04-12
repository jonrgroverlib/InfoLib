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
using System;                         // for 
using System.Text.RegularExpressions; // for Regex

namespace InfoLib.Winforms
{
    // --------------------------------------------------------------------------------------------
    /// <!-- WidgetList -->
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>stub?</remarks>
    public class WidgetList : IWidgetIterator
    {
        private int _currentIndex;
        //private object _currentObject;


       public void First()
        {
            _currentIndex = 0;
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        public void Next()
        {
            _currentIndex++;
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        public bool HasNext()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }
    }
}
