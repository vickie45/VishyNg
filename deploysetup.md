Deploying an Angular app in IIS involves several steps and configuration settings to ensure the app runs correctly. Here's a detailed guide on how to set up and configure your Angular app for deployment on an IIS server.

### Step 1: Build the Angular App
First, build your Angular app for production using the `ng build` command. This command creates a production-ready version of your app.

#### Command to Build:

1. If your app is being hosted at the **root** of your site:
    ```bash
    ng build --prod --base-href /
    ```

2. If your app is being hosted in a **subdirectory** (e.g., `http://10.65.15.86/my-app/`):
    ```bash
    ng build --prod --base-href /my-app/
    ```

After running the build, the output will be placed in the `dist/your-app-name` folder.

### Step 2: Set Up IIS Server

1. **Open IIS Manager**:
   - Type `inetmgr` in the Windows search bar to open IIS Manager.

2. **Create a New Site in IIS**:
   - Right-click on **Sites** in the Connections pane and choose **Add Website**.
   - Fill in the site details:
     - **Site Name**: Choose a name for your site (e.g., `AngularApp`).
     - **Physical Path**: Point this to the folder containing the Angular build output (`dist/your-app-name`).
     - **Binding**: Choose the correct IP address, port (80 for HTTP or 443 for HTTPS), and hostname (if applicable).

3. **Set Application Pool**:
   - Ensure the **Application Pool** is using **Integrated Pipeline** and is targeting the appropriate .NET version.
   - For Angular apps, **No Managed Code** should also work fine since you're serving static files.

### Step 3: Configure `web.config` for Angular Routing

You need to configure IIS to properly serve the Angular app and handle client-side routing. Place the following `web.config` file in the root of your Angular app's build folder (`dist/your-app-name`).

#### Sample `web.config`:
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <system.webServer>
    <rewrite>
      <rules>
        <!-- Angular routes handling -->
        <rule name="Angular Routes" stopProcessing="true">
          <match url=".*" />
          <conditions logicalGrouping="MatchAll">
            <add input="{REQUEST_FILENAME}" matchType="IsFile" negate="true" />
            <add input="{REQUEST_FILENAME}" matchType="IsDirectory" negate="true" />
          </conditions>
          <action type="Rewrite" url="/index.html" />
        </rule>
      </rules>
    </rewrite>
    <!-- Enable GZIP compression for performance optimization -->
    <httpCompression>
      <dynamicTypes>
        <add mimeType="text/*" enabled="true"/>
        <add mimeType="application/javascript" enabled="true"/>
        <add mimeType="application/json" enabled="true"/>
      </dynamicTypes>
      <staticTypes>
        <add mimeType="text/*" enabled="true"/>
        <add mimeType="application/javascript" enabled="true"/>
        <add mimeType="application/json" enabled="true"/>
        <add mimeType="application/font-woff2" enabled="true"/>
      </staticTypes>
    </httpCompression>
    <!-- MIME types to ensure all Angular files are served -->
    <staticContent>
      <mimeMap fileExtension=".json" mimeType="application/json" />
      <mimeMap fileExtension=".woff2" mimeType="font/woff2" />
      <mimeMap fileExtension=".woff" mimeType="font/woff" />
      <mimeMap fileExtension=".ttf" mimeType="font/ttf" />
    </staticContent>
  </system.webServer>
</configuration>
```

### Explanation of `web.config`:
- **Rewrite Rule**: Redirects all incoming requests to `index.html` for Angular's client-side routing, unless the requested URL matches an existing file or directory.
- **MIME Types**: Ensures that the server correctly serves all necessary static assets, including JavaScript, JSON, fonts, etc.
- **HTTP Compression**: Optionally, enables GZIP compression to improve performance by reducing the size of the transferred files.

### Step 4: Ensure MIME Types Are Correctly Configured in IIS

MIME types help IIS serve the necessary file types for your Angular app. You can add MIME types manually in IIS:

1. In **IIS Manager**, select your site.
2. Open **MIME Types** in the **Features View**.
3. Ensure that the following MIME types are configured:
   - `.js` → `application/javascript`
   - `.json` → `application/json`
   - `.woff2` → `font/woff2`
   - `.woff` → `font/woff`
   - `.html` → `text/html`
   - `.css` → `text/css`

### Step 5: Set Default Document to `index.html`

You need to ensure that `index.html` is the default document so that requests to the root URL (e.g., `http://10.65.15.86/`) serve the Angular app.

1. In **IIS Manager**, select your site.
2. Open **Default Document**.
3. Ensure `index.html` is listed as a default document. If it’s not, click **Add** and type `index.html`.

### Step 6: Bind Domain or Hostname (Optional)

If you're using a domain name (e.g., `http://my-angular-app.com`), configure the **Bindings** in IIS:

1. Right-click the site in **IIS Manager** and choose **Edit Bindings**.
2. Add a new binding for the domain, or modify the existing binding to include your domain name.

### Step 7: Access the App

After everything is set up, you can access your Angular app through a browser:

1. **If hosted on the root** (port 80):
   - `http://10.65.15.86/` (or `http://localhost` on the server itself).

2. **If hosted in a subdirectory** (port 80):
   - `http://10.65.15.86/my-app/`

Ensure the URL reflects your actual setup.

### Step 8: Troubleshooting

- **404 Errors**: Make sure the rewrite rules in `web.config` are correctly set and that the physical path in IIS points to the correct folder.
- **Missing Static Files**: Ensure that the correct MIME types are configured, and that static files like JavaScript, CSS, and fonts are served.
- **Caching Issues**: Clear your browser cache or try accessing the app in an incognito/private window after deploying.

With these steps, your Angular app should now be deployed and running on an IIS server. Let me know if you encounter any further issues!