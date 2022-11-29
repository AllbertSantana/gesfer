import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpInterceptor,
  HttpResponse
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { finalize, tap } from 'rxjs/operators';
import { MessageService } from 'src/app/shared/services/message/message.service';
import { Message, MessageIcon, MessageTypes } from 'src/app/shared/model/message';

@Injectable()
export class LoggingInterceptor implements HttpInterceptor {

  constructor(private messenger: MessageService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler) {
    
    return next.handle(req)
      .pipe(
        tap({
          next: () => {},
          error: (error) => {
            const msg: Message = {
              title: `Houve um erro com sua requisição.`,
              type: MessageTypes.error,
            };
  
            this.messenger.notify(msg);
          }
        }),
      );
  }
}
