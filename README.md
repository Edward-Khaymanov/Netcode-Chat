# Netcode-chat
Chat system built on unity Netcode for GameObjects with command and voting support

## Requirements
- Textmeshpro
- Netcode for GameObjects

## Preview

![Client1](https://github.com/Edward-Khaymanov/Netcode-Chat/assets/104985307/3e74e57b-3d6e-447b-b75d-c3fb493f14cc)
![Client2](https://github.com/Edward-Khaymanov/Netcode-Chat/assets/104985307/0d5ffa64-ef1f-4580-8a10-46977b36a684)

***

![Client2Cringe](https://github.com/Edward-Khaymanov/Netcode-Chat/assets/104985307/6521378d-85cc-4220-84b4-c026072dcb5f)
![Client2Kick](https://github.com/Edward-Khaymanov/Netcode-Chat/assets/104985307/225d5f57-d445-4c54-b59b-87c5510dacbe)

## Configure
In `ChatHandler` prefab you can see these options

![ChatHandler](https://github.com/Edward-Khaymanov/Netcode-Chat/assets/104985307/8cbbee46-209b-4c4a-951c-7b71a31e1c73)

- SystemMessageColor - when the system sends a message, the nickname is displayed in this color
- OwnerMessageColor - when a user sends a message, his nickname is displayed in this color
- ShowOnJoinMessage - should the system notify everyone about the user joining
- ShowOnLeaveMessage - should the system notify everyone about the user leaving
- OnJoinMessage - message after user nickname on join
- OnOnLeaveMessage - message after user nickname on leave

## Example
In `Example` folder you can see `ExampleScene` scene.
To test, you have to build the project twice with different "IsHost" parameters. <br/>
Or use the [ParrelSync](https://github.com/VeriorPies/ParrelSync) extension and modify the `Example` script like this

**Before:**
```C#

private void Start()
{
    if (_isHost == false)
    {
        NetworkManager.Singleton.StartClient();
        return;
    }
    //...
    //...
    //...
}

```

**After:**
```C#

private void Start()
{
    if (ClonesManager.GetArgument() == "client")
        _isHost = false;
    else
        _isHost = true;
    
    if (_isHost == false)
    {
        NetworkManager.Singleton.StartClient();
        return;
    }
    //...
    //...
    //...
}

```
