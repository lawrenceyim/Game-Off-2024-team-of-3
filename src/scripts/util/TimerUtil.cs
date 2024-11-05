using Godot;

/*
    This class exists solely because I got tired of writing this:
    timer = new Timer();
    AddChild(timer);
*/
public class TimerUtil {
    public static Timer CreateTimer(Node node) {
        Timer timer = new Timer();
        node.AddChild(timer);
        return timer;
    }
}