/*
    Still deciding if it's better to explicitly define an end node or use null/absence of node as a terminator for dialogue tree.
*/

public class EndNode : BaseNode {
    public override string GetNextNodeKey() {
        return null;
    }
}