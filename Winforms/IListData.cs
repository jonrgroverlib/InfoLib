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

namespace InfoLib.Winforms
{
    // --------------------------------------------------------------------------------------------
    /// <!-- IListData -->
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>stub?</remarks>
    public interface IListData
    {
        string ID    { get; set; }  // unique key
        string Code  { get; set; }  // natural key
        string Descr { get; set; }  // display text
    }
}
