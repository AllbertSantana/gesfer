import {inject} from '@angular/core';
import { ActivatedRouteSnapshot, CanActivateChildFn, CanActivateFn, NavigationExtras, Router, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '../services/auth/auth.service';
import { MessageService } from '../services/message/message.service';
import { MessageTypes } from '../model/message';

export const authenticationGuard: CanActivateChildFn = (childRoute: ActivatedRouteSnapshot, state: RouterStateSnapshot) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isLoggedIn) {
    return true;
  }
  
  authService.redirectUrl = state.url;
  return router.parseUrl('/login');
};

export const authorizationGuard: CanActivateFn = (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => {
  const authService = inject(AuthService);
  const messenger = inject(MessageService);
  const router = inject(Router);

  if (authService.decodedToken?.Perfil === 'Administrador') {
    return true;
  }

  messenger.notify({
    title: 'Você não tem permissão para acessar esta página.',
    type: MessageTypes.warning
  });

  // Redirect to the home page
  return router.parseUrl('/');
};

export const loginGuard: CanActivateFn = (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => {
  const authService = inject(AuthService);
  const messenger = inject(MessageService);
  const router = inject(Router);

  if (authService.isLoggedIn) {
    messenger.notify({
      title: 'Você já foi identificado como um usuário.',
      type: MessageTypes.info
    });

    return false;
  }

  return true;
}