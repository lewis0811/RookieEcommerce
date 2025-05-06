import { UserManager, WebStorageStateStore, User, UserManagerSettings } from 'oidc-client-ts';

export const oidcConfig: UserManagerSettings = {
  authority: import.meta.env.VITE_AUTHORITY,
  client_id: import.meta.env.VITE_CLIENT_ID,
  client_secret: import.meta.env.VITE_CLIENT_SECRET,
  redirect_uri: import.meta.env.VITE_REDIRECT_URI,
  post_logout_redirect_uri: import.meta.env.VITE_POST_LOGOUT_REDIRECT_URI,
  response_type: 'code',
  scope: 'openid profile email api roles',
  filterProtocolClaims: true,
  loadUserInfo: true,
  automaticSilentRenew: true,
  includeIdTokenInSilentRenew: true,
  userStore: new WebStorageStateStore({ store: window.localStorage })
};

export const userManager = new UserManager(oidcConfig);

export class AuthService {
  public async login(): Promise<void> {
    return userManager.signinRedirect();
  }

  public async logout(): Promise<void> {
    const user = await userManager.getUser();
    const idToken = user?.id_token;
    return userManager.signoutRedirect({ id_token_hint: idToken });
  }

  public async handleLoginCallback(): Promise<User | null> {
    try {
      const user = await userManager.signinRedirectCallback();
      console.log('User logged in:', user);
      return user;
    } catch (error) {
      console.error('Error handling login callback:', error);
      return null;
    }
  }

  public async handleLogoutCallback(): Promise<void> {
    try {
      await userManager.signoutRedirectCallback();
      console.log('User logged out');
    } catch (error) {
      console.error('Error handling logout callback:', error);
    }
  }

  public async getUser(): Promise<User | null> {
    return userManager.getUser();
  }

  public async getAccessToken(): Promise<string | null> {
    const user = await this.getUser();
    return user?.access_token ?? null;
  }

  public async isAuthenticated(): Promise<boolean> {
    const user = await this.getUser();
    return !!user && !user.expired;
  }

  public async clearStaleState(): Promise<void> {
    return userManager.clearStaleState();
  }

  public async renewToken(): Promise<User | null> {
    try {
      const user = await userManager.signinSilent();
      console.log('Token renewed silently:', user);
      return user;
    } catch (error) {
      console.error('Silent renew failed:', error);
      return null;
    }
  }

  // Listen for token expiration events
  public registerEvents(): void {
    userManager.events.addAccessTokenExpired(() => {
      console.log('Access token expired, attempting silent renew...');
      this.renewToken().catch((err) => {
        console.error("Error during automatic silent renew:", err);
        this.logout();
      });
    });

    userManager.events.addUserLoaded((user) => {
      console.log('User loaded:', user);
    });

    userManager.events.addUserUnloaded(() => {
      console.log('User unloaded/logged out');
    });

     userManager.events.addSilentRenewError((error) => {
      console.error('Silent renew error:', error);
    });

    userManager.events.addUserSignedOut(() => {
       console.log('User signed out from token expiration or other means');
       this.clearStaleState();
    });
  }
}

export const authService = new AuthService();
authService.registerEvents();
authService.clearStaleState().catch(console.error);