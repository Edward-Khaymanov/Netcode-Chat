<p align="center">
    <a href="./LICENSE">
        <img src="https://img.shields.io/github/license/Edward-Khaymanov/Netcode-Chat?label=license&style=for-the-badge">
    </a>
</p>

# Netcode-chat
Chat system built on unity Netcode for GameObjects with command and voting support

## Requirements
- Textmeshpro
- Netcode for GameObjects

## Preview

![Client1](https://github.com/Edward-Khaymanov/Netcode-Chat/assets/104985307/1bcfbfb8-3e77-4d2c-88e9-274e013d5ea2)
![Client2](https://github.com/Edward-Khaymanov/Netcode-Chat/assets/104985307/56f5ecaf-5d66-4006-9665-41d3fae488dd)

***

![Client2Cringe](https://github.com/Edward-Khaymanov/Netcode-Chat/assets/104985307/00accc9b-8b80-4c96-a2a9-9f36d442700b)
![Client2Kick](https://github.com/Edward-Khaymanov/Netcode-Chat/assets/104985307/dd13edc2-5190-4816-8673-cfc9d583e876)

## Configure
In `ChatHandler` prefab you can see these options

![ChatHandler](https://github.com/Edward-Khaymanov/Netcode-Chat/assets/104985307/41a786e9-22c1-43dd-b033-f90bd284148e)

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
