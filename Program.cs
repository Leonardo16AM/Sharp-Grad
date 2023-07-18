
var v= dataset.get_dataset(400);
Console.WriteLine("Dataset:");
scatter(v);



MLP cerebrin = new MLP(2,new List<int>{8,1});

List<value> X = new List<value>();
X.Add(new value(v[0].X[0],"a"));
X.Add(new value(v[0].X[1],"b"));
List<value> Y = cerebrin.forward(X);


Console.WriteLine(Y[0].data);



int epochs=1000;
double lr=0.0000000001;

for(int i=0;i<epochs;i++){
    Console.WriteLine("Epoch: "+i);
    value loss=new value(0.0,"loss");
    List<dataset.data>preds= new List<dataset.data>();

    for(int j=0;j<v.Count;j++){
        X = new List<value>();
        X.Add(new value(v[j].X[0],"a"));
        X.Add(new value(v[j].X[1],"b"));
        Y = cerebrin.forward(X);
        List<value> Ygt = new List<value>();
        Ygt.Add(new value(v[j].Y[0],"c"));
        var nl = loss + MSE(Y,Ygt);
        loss=nl;

        int val;
        if( Math.Abs(Y[0].data-1) < Math.Abs(Y[0].data-2) ){
            val=1;
        }else{
            val=2;
        }
        dataset.data nd= new dataset.data(v[j].X,new List<int>{val});
        preds.Add(nd);
    }
    
    loss.grad=1.0;
    loss.backpropagate();
    foreach(layer l in cerebrin.layers){
        foreach(neuron n in l.neurons){
            foreach(value w in n.W){
                // Console.WriteLine(w.data);
                w.data=w.data-lr*w.grad;
            }
            n.B.data=n.B.data-lr*n.B.grad;
        }
    }

    loss.reset_grad();  

    Console.WriteLine("Loss: "+loss.data);
    scatter(preds);
    Console.WriteLine("Press any key to continue...");
    // Console.ReadKey();
}




value MSE(List<value> Y, List<value> Y_hat){
    value loss = new value(0.0,"loss");
    for(int i=0;i<Y.Count;i++){
        var nl = loss + ((Y[i]-Y_hat[i])^(new value(2.0,"w")));
        loss=nl;
    }
    return loss;
}














void scatter(List<dataset.data>v){
    int[,] mat= new int[300,300];
    for(int i=0;i<v.Count;i++){
        mat[v[i].X[0]+15,v[i].X[1]+15]=v[i].Y[0];
    }
    Console.Write("╔");
    for(int i=0;i<30;i++){
        Console.Write("═");
    }
    Console.WriteLine("╗");
    for(int i=0;i<30;i++){
        Console.Write("║");
        for(int j=0;j<30;j++){
            if(mat[i,j]==0){
                Console.Write(" ");
            }
            if(mat[i,j]==1){
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("o");
            }
            if(mat[i,j]==2){
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("o");
            }
            Console.ResetColor();
        }
        Console.WriteLine("║");
    }
    
    Console.Write("╚");
    for(int i=0;i<30;i++){
        Console.Write("═");
    }
    Console.WriteLine("╝");
}

static class dataset{
        
    public class data{
        public List<int>X;
        public List<int>Y;

        public data(List<int> X, List<int> Y){
            this.X=X;
            this.Y=Y;
        }
    }

    public static List<data> get_dataset(int n){
        var rand = new Random(); 
        List<data> dataset = new List<data>();
        for(int i=0;i<n;i++){
            List<int> X = new List<int>();
            List<int> Y = new List<int>();
            X.Add(rand.Next(-15,15));
            X.Add(rand.Next(-15,15));
            int x=X[0];
            int y=X[1];
            if(  y>2*x+5 )
                Y.Add(1);
            else
                Y.Add(2);
            
            dataset.Add(new data(X,Y));
        }
        return dataset;
    }
}







value a = new value(1.5,"a");
value b = new value(2.0,"b");
value c = new value(6.0,"b");

// value d=(a+b*c);
// value e=d/(new value(2.0,"2"));
// value f=e^(new value(2.0,"2"));
// value g=f.relu();   

// g.grad=1.0;
// g.backpropagate();

// Console.WriteLine(a.grad);
// Console.WriteLine(b.grad);
// Console.WriteLine(c.grad);

// value j= new value(0.5,"j");
// value k= j.tanh();

// k.grad=1.0;
// k.backpropagate();
// Console.WriteLine(j.grad);
// Console.WriteLine(k.data);


// MLP cerebrin = new MLP(4,new List<int>{4,16,16,1});

// List<value> X = new List<value>();
// X.Add(new value(1.0,"x1"));
// X.Add(new value(2.0,"x2"));
// X.Add(new value(3.0,"x3"));
// X.Add(new value(4.0,"x4"));

// List<value> Y = cerebrin.forward(X);
// Console.WriteLine(Y[0].data);