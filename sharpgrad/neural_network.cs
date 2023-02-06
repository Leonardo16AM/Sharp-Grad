public class neuron{
    public List<value>W;
    public value B;
    public int inputs;
    public bool act_func;
    public Random rand = new Random();

    public neuron(int inputs,bool act_func){
        this.W = new List<value>();
        this.B=new value(rand.NextDouble(), "B",new List<value>{});
        this.inputs=inputs;
        this.act_func=act_func;
        for(int i=0;i<inputs;i++){
            this.W.Add(new value(rand.NextDouble(), $"W{i}",new List<value>{}));
        }
    }
    public value forward(List<value> X){
        value sum = new value(0.0, "sum",new List<value>{});
        for(int i=0;i<this.inputs;i++){
            sum= sum + X[i]*W[i];
        }
        sum+=this.B;
        if(this.act_func){
            return value.relu(sum);
        }else{
            return sum;
        }
    }
    public void reset_grad(){
        foreach(value w in this.W){
            w.grad=0.0;
        }
        this.B.grad=0.0;
    }
}

public class layer{
    public List<neuron> neurons;
    public int neurons_count;
    public int inputs;
    public bool act_func;

    public layer(int neurons, int inputs, bool act_func){
        this.neurons = new List<neuron>();
        for(int i=0;i<neurons;i++){
            this.neurons.Add(new neuron(inputs,act_func));
        }
    }

    public List<value> forward(List<value> X){
        List<value> Y = new List<value>();
        foreach(neuron n in this.neurons){
            Y.Add(n.forward(X));
        }
        return Y;
    }
    public void reset_grad(){
        foreach(neuron n in this.neurons){
            n.reset_grad();
        }
    }
}

public class MLP{
    public List<layer> layers;
    public int inputs;

    public MLP(int inputs,List<int> outputs){
        this.layers = new List<layer>();
        this.inputs=inputs;
        this.layers.Add(new layer(outputs[0],inputs,false));
        for(int i=1;i<outputs.Count;i++){
            this.layers.Add(new layer(outputs[i],outputs[i-1],true));
        }
    }

    public List<value> forward(List<value> X){
        List<value> Y = new List<value>();
        foreach(layer l in this.layers){
            Y=l.forward(X);
            X=Y;
        }
        return X;
    }

    public void reset_grad(){
        foreach(layer l in this.layers){
            l.reset_grad();
        }
    }
}