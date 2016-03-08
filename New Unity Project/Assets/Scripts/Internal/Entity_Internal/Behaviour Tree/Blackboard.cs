using UnityEngine;
using System.Collections.Generic;

namespace BehaviourTree
{
    public sealed class TreeMemory
    {
        public Memory treeMemory = new Memory();
        public Dictionary<Node, Memory> nodeMemory = new Dictionary<Node, Memory>();
    }
    public sealed class Memory
    {
        public Dictionary<string, object> memory = new Dictionary<string, object>();
        public void Set(string key, object value)
        {
            if (!memory.ContainsKey(key))
                memory.Add(key, null);
            memory[key] = value;
        }
        public T Get<T>(string key)
        {
            if (memory.ContainsKey(key))
                return (T)memory[key];
            return default(T);
        }
    }

    public class Blackboard
    {

        public GameObject agent;
        private Dictionary<Tree, TreeMemory> treeMemory = new Dictionary<Tree, TreeMemory>();

        public Blackboard(GameObject agent)
        {
            this.agent = agent;
        }
        private TreeMemory GetTreeMemory(Tree treeScope)
        {
            TreeMemory mem;

            if (!treeMemory.ContainsKey(treeScope))
            {
                treeMemory.Add(treeScope, new TreeMemory());
            }
            mem = treeMemory[treeScope];

            return mem;
        }
        private Memory GetNodeMemory(Tree treeScope, Node nodeScope)
        {
            TreeMemory treeMem = GetTreeMemory(treeScope);
            Memory nodeMem;

            if (!treeMem.nodeMemory.ContainsKey(nodeScope))
            {
                treeMem.nodeMemory.Add(nodeScope, new Memory());
            }
            nodeMem = treeMem.nodeMemory[nodeScope];
            return nodeMem;
        }
        /*
        private void GetMemory(BehaviourTree treeScope, Node nodeScope)
        {

        }
        */
        public T Get<T>(string key, Tree treeScope, Node nodeScope)
        {
            Memory nodeMem = GetNodeMemory(treeScope, nodeScope);
            return nodeMem.Get<T>(key);
        }
        public TreeMemory Get(Tree treeScope)
        {
            TreeMemory mem = GetTreeMemory(treeScope);
            return mem;
        }
        public void Set(string key, object value, Tree treeScope, Node nodeScope)
        {
            TreeMemory mem = GetTreeMemory(treeScope);
            mem.nodeMemory[nodeScope].Set(key, value);
        }
        public void Set(string key, object value, Tree treeScope)
        {
            TreeMemory mem = GetTreeMemory(treeScope);
            mem.treeMemory.Set(key, value);
        }
    }
}
