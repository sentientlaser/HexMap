using UnityEngine;
using System.Collections;

interface Event { }

interface Publisher {
    
}

interface Subscriber {
    void subscribe(Publisher p);
    void onEvent(Event e);
}
