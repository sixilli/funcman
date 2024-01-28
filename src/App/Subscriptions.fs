namespace App

module Subscriptions =
    open Main
    let registerItemStore(observer : ItemStore.Observer) (state: State) =
        let itemsChanged (observer : ItemStore.Observer) dispatch =
            observer.ItemsChanged.Subscribe(fun items -> dispatch (ItemStoreUpdate items))

        let selectedChanged (observer : ItemStore.Observer) dispatch =
            observer.SelectedChanged.Subscribe(fun request -> dispatch (ItemStoreUpdate request))

        [
            [ nameof itemsChanged ], itemsChanged observer
            [ nameof selectedChanged ], selectedChanged observer
        ]
    
    
