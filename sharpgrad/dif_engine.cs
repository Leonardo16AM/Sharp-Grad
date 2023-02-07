public class value{
    public double data,grad;
    public List<value> children,topo_sort;
    public string name;
    public delegate void backward_pass();
    public backward_pass backward;
    HashSet<value> visited;
    
    public value(double data,string name , List<value> children=null){
        this.data = data;
        this.grad=0.0;
        this.children = children;
        this.name=name;
        this.backward=new backward_pass(backward_empt);
        this.topo_sort=new List<value>();
    }
    void backward_empt(){return;}


    /* BASIC ARITHMETIC OPERATIONS */

    public void backward_add(){
        foreach(value child in children){
            child.grad += grad; 
        }
    }
    public static value operator +(value a, value b){
        value c = new value(a.data+b.data,"+", new List<value>{a,b});
        c.backward = new backward_pass(c.backward_add);
        return c;
    }

    public void backward_subs(){
        children[0].grad+=grad;
        children[1].grad-=grad;
    }
    public static value operator -(value a, value b){
        value c = new value(a.data-b.data,"-", new List<value>{a,b});
        c.backward = new backward_pass(c.backward_subs);
        return c;
    }

    public void backward_mul(){
        children[0].grad+=grad*children[1].data;
        children[1].grad+=grad*children[0].data;
    }
    public static value operator *(value a, value b){
        value c = new value(a.data*b.data,"*", new List<value>{a,b});
        c.backward = new backward_pass(c.backward_mul);
        return c;
    }
    
    public void backward_pow(){
        children[0].grad += children[1].data * Math.Pow(children[0].data,(children[1].data-1.0)) * grad;
        children[1].grad += Math.Pow(children[0].data,children[1].data) * Math.Log(children[0].data) * grad;
    }
    public static value operator ^(value a, value b){
        value c = new value(Math.Pow(a.data,b.data), "^",new List<value>{a,b});
        c.backward = new backward_pass(c.backward_pow);
        return c;
    }

    public void backward_new(){
        children[0].grad += grad;
    }

    public static value operator /(value a, value b){
        value c= a*(b^(new value(-1,"-1")));
        value ret=new value(c.data,"/",new List<value>{c});
        ret.backward = new backward_pass(ret.backward_new);
        return ret;
    }


    /* ACTIVATION FUNCTIONS */

    public void backward_relu(){
        children[0].grad+=Convert.ToDouble((grad>0))*grad;
    }
    public value relu(){
        value c = new value((data<=0)?(0):(data), "ReLU", new List<value>{this});
        c.backward = new backward_pass(c.backward_relu);
        return c;
    }

    public value tanh(){
        value e= new value(Math.E,"e");
        value la=(new value(0.0,"zero"))-this;
        value c = (((e^this)-(e^la))/((e^this)+(e^la)));
        value ret=new value(c.data,"tanh",new List<value>{c});
        ret.backward = new backward_pass(ret.backward_new);
        return c;
    }

    












    /* BACKPROPAGATION*/
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

