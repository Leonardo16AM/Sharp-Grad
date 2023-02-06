public class value{
    public double data,grad;
    public List<value> children;
    public string name;
    public delegate void backward_pass();
    public backward_pass backward;
    public List<value> topo_sort;
    

    public value(double data,string name , List<value> children=null){
        this.data = data;
        this.grad=0.0;
        this.children = children;
        this.name=name;
        this.backward=new backward_pass(backward_empt);
        this.topo_sort=new List<value>();
    }
    void backward_empt(){return;}


    public void backward_add(){
        foreach(value child in this.children){
            child.grad += this.grad;
        }
    }
    public static value operator +(value a, value b){
        value c = new value(a.data+b.data,"+", new List<value>{a,b});
        c.backward = new backward_pass(c.backward_add);
        return c;
    }

    public void backward_mul(){
        this.children[0].grad+=this.grad*this.children[1].data;
        this.children[1].grad+=this.grad*this.children[0].data;
    }
    public static value operator *(value a, value b){
        value c = new value(a.data*b.data,"*", new List<value>{a,b});
        c.backward = new backward_pass(c.backward_mul);
        return c;
    }
    
    public void backward_pow(){
        this.children[0].grad+=this.children[1].data * Math.Pow(this.children[0].data,(this.children[1].data-1.0)) * this.grad;
    }
    public static value operator ^(value a, value b){
        value c = new value(Math.Pow(a.data,b.data), "^",new List<value>{a,b});
        c.backward = new backward_pass(c.backward_pow);
        return c;
    }

    public void backward_relu(){
        this.children[0].grad+=Convert.ToDouble((this.grad>0))*this.grad;
    }
    public static value relu(value a){
        value c = new value((a.data<=0)?(0):(a.data), "ReLU", new List<value>{a});
        c.backward = new backward_pass(c.backward_relu);
        return c;
    }

    HashSet<value> visited;
    void dfs(value u){
        visited.Add(u);
        if(u.children==null){topo_sort.Add(u);return;}
        foreach(value v in u.children){
            if(visited.Contains(v)){continue;} 
            dfs(v);
        }
        topo_sort.Add(u);
    }
    public void backpropagate(){
        visited= new HashSet<value>();
        dfs(this);
        topo_sort.Reverse();
        foreach(value u in topo_sort){
            u.backward();
        }  
    }
}   

