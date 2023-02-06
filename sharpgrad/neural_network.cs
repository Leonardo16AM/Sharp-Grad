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
}

public class layer{
    // public List<neuron> neurons;
    // public int neurons_count;
    // public int inputs;
    // public bool act_func;

    // public layer(int neurons, int inputs, bool act_func){
    //     this.neurons = new List<neuron>();
    //     for(int i=0;i<neurons;i++){
    //         this.neurons.Add(new neuron(inputs,act_func));
    //     }
    // }


}

public class MLP{


}