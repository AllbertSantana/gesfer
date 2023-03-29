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

export const modifyAuthorizationGuard: CanActivateFn = (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => {
  const authService = inject(AuthService);
  const messenger = inject(MessageService);
  const router = inject(Router);
  const permissions = [
    "RemoverFuncionario",
    "RemoverFuncionarios",
    "CriarFuncionario",
    "EditarFuncionario",
    "RemoverFerias",
    "CriarFerias",
    "EditarFerias"
  ];

  if (
    permissions.every(p => authService.userInfo?.permissoes.includes(p))
  ) {
    return true;
  }

  messenger.notify({
    title: 'Você não tem permissão para acessar esta página.',
    type: MessageTypes.warning
  });

  return router.parseUrl('/');
};

export const adminAuthorizationGuard: CanActivateFn = (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => {
  const authService = inject(AuthService);
  const messenger = inject(MessageService);
  const router = inject(Router);
  const permissions = [
    "BuscarUsuario",
    "ListarUsuarios",
    "RemoverUsuario",
    "CriarUsuario",
    "EditarUsuario"
  ];

  if (
    permissions.every(p => authService.userInfo?.permissoes.includes(p))
  ) {
    return true;
  }

  messenger.notify({
    title: 'Você não tem permissão para acessar esta página.',
    type: MessageTypes.warning
  });

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