/*How to edit hotkeys:
From Unity Documentation
    To create a hotkey you can use the following special characters: 
        % (ctrl on Windows, cmd on macOS), 
        # (shift)
        & (alt). 
        _ (No special modifier key combinations are required)

    For example to create a menu with hotkey shift-alt-g use "#&g". 
    To create a menu with hotkey g and no key modifiers pressed use "_g".
*/

namespace EasyTextureAssign
{
    public class Hotkeys_ET
    {
        public const string ET_ASSIGN_HOTKEY = "&t";
        public const string ET_CREATE_HOTKEY = "%&c";
        public const string ET_CLEAR_HOTKEY = "#&c";
    }
}
