using System.Threading.Tasks;

namespace Shuut.Scripts;

public class InputBuffer
{
    public bool IsUsed => InputUsed is not null;
    public string InputUsed { get; set; }
    public int TimeMs { get; init; }

    private Task toAwait;
	
    public async void Use(string n)
    {
        if (IsUsed)
        {
            return;
        }
        InputUsed = n;
        toAwait = Task.Delay(TimeMs);
        await toAwait;
        InputUsed = null;
    }

    public void Reset()
    {
        InputUsed = null;
    }
    
    
}