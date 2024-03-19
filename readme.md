# unity-webview-sample-app

This project demonstrates how to integrate the Verisoul platform webview and API into a Unity project

## Getting Started

Ensure you have the following details:

- Unity version `2021.3.25f1`
- [UniWebView](https://docs.uniwebview.com/) plugin installed
- A Verisoul API Key
- A Verisoul Project ID

### Running the Demo

1. Open the sample scene in Unity.
2. Click on the `Session Manager` game object in the hierarchy.
3. In the Inspector panel, configure the following fields:
    - `Environment`: Choose the environment for your project.
    - `Project ID`: Enter your project ID.
    - `API Key`: Enter your API key.
4. Press the `Play` button to run the demo.
  - The demo cannot be run in Windows Unity Editor. You will need to build and run the project on a Mac, an Android or iOS device.
  - Use the LogCat plugin to view the logs on Android.
5. Click on the `Start Session` button to initiate a session.
6. Enter your `Account ID` in the provided field.
7. Click on the `Authenticate` button to authenticate the session.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details