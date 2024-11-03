/*
    GetNextNodeKey should return a string that is used as a key to find the next node
    in a dictionary with key-value of <string, BaseNode>.

    Do not use null values. An empty string of "" should be used to indicate the absence of the next node.
*/
public abstract class BaseNode {
    public abstract string GetNextNodeKey();
}