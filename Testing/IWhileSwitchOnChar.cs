using System.Text;

namespace InfoLib.Testing
{
    // --------------------------------------------------------------------------------------------
    /// <!-- IWhileSwitchOnChar -->
    /// <summary>
    ///      An interface for the following usage of an instance:
    ///      while(instance) swith(instance) { case 'A': code; break; case 'B': code; break; ... }
    ///      when used with the shortcuts at the end
    /// </summary>
    /// <remarks>
    ///      I'm using properties to make this all syntactic sugary
    ///      
    /// 
    ///      Usage Examples:
    ///      
    ///           
    ///          Endeme hiworld = "ABCDEFGHIPO";
    ///      
    ///          while (hiworld.Run) switch (hiworld.Entry)
    ///          {
    ///              case 'A': hiworld.Skip(3); break;
    ///              case 'B': break;
    ///              case 'C': hiworld.Break(); break;
    ///          }
    ///      
    /// 
    ///          // ---------------------------------------------------------
    ///          //  With the implicit bool and char operators included
    ///          // ---------------------------------------------------------
    ///          while (hiworld) switch (hiworld)
    ///          {
    ///              case 'A': hiworld.Skip(3); break;
    ///              case 'B': break;
    ///              case 'C': hiworld.Break(); break;
    ///          }
    ///      
    /// 
    ///          // ---------------------------------------------------------
    ///          //  To also use return to break out of the while loop
    ///          // ---------------------------------------------------------
    ///          Action work = delegate { while (hiworld) switch (hiworld)
    ///          {
    ///              case 'A': hiworld.Skip(3); break;
    ///              case 'B': break;
    ///              case 'C': hiworld.Break(); break;
    ///              case 'D': return;
    ///          } };
    ///      
    /// 
    /// 
    ///       Implementation Example:
    ///       
    ///           public class Endeme : IWhileSwitchChar
    ///           {
    ///               public List&lt;char> _string { get; set; }       
    ///               
    /// 
    ///               public void Break() { Cursor = _string.Count; }                                                                    
    ///               public int  Cursor  { get; private set; }                                                                    
    ///               public char Entry   { get { if (_string.Count > Cursor) { return _string[Cursor]; } else {              return '\r' ; } } }
    ///               public bool Run     { get { if (_string.Count > Cursor) { Cursor++; return true ; } else { Cursor = -1; return false; } } }
    ///               public void Skip(int fwd) { Cursor += fwd;    }
    ///               public static implicit operator bool(Endeme en) { if (en == null) return false; return en.Run  ; }
    ///               public static implicit operator char(Endeme en) { if (en == null) return '\r' ; return en.Entry; }
    ///           }
    /// 
    /// </remarks>
    interface IWhileSwitchOnChar
    {
                              /// <summary>leave the while loop</summary>
        void Break  ()     ;  /// <summary>Current position in the sequence</summary>
        int  Cursor { get; }  /// <summary>current switch value in the sequence</summary>
        char Step   { get; }  /// <summary>start,continue or stop running the sequence</summary>
        bool Run    { get; }  /// <summary>skip forward a number of steps</summary>
        void Skip   (int fwd);

        //  Shortcuts like the following may be handy:
      //public static implicit operator bool(Endeme en) { if (en == null) return false; return en.Run  (); }
    }
}
