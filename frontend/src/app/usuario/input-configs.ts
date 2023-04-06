import { Validators } from "@angular/forms";
import { FormType, InputConfig, InputType, InputValidator, ValidatorType } from "../shared/model/form";

export const initialInputsConfigs: InputConfig[] = [
    {
        control: {
            name: 'nome',
            value: '',
            isDisabled: false,
            usedAt: [FormType.add, FormType.edit, FormType.filter],
            validators: [
                { type: ValidatorType.required, message: 'Campo Obrigatório', validator: Validators.required }
            ] as InputValidator[]
        },
        view: {
            label: 'Nome',
            type: InputType.textspan,
        }
    },
    {
        control: {
            name: 'cpf',
            value: '',
            isDisabled: false,
            usedAt: [FormType.add, FormType.edit, FormType.filter],
            validators: [
                { type: ValidatorType.required, message: 'Campo Obrigatório', validator: Validators.required }
            ] as InputValidator[]
        },
        view: {
            label: 'CPF',
            type: InputType.textspan,
            mask: [/\d/, /\d/, /\d/, '.', /\d/, /\d/, /\d/, '.', /\d/, /\d/, /\d/, '-', /\d/, /\d/]
        }
    },
    {
        control: {
            name: 'email',
            value: '',
            isDisabled: false,
            usedAt: [FormType.add, FormType.edit, FormType.filter],
            validators: [
                { type: ValidatorType.required, message: 'Campo Obrigatório', validator: Validators.required }
            ] as InputValidator[]
        },
        view: {
            label: 'Email',
            type: InputType.textspan,
        }
    },
    {
        control: {
            name: 'perfil',
            value: '',
            isDisabled: false,
            usedAt: [FormType.add, FormType.edit, FormType.filter],
            validators: [
                { type: ValidatorType.required, message: 'Campo Obrigatório', validator: Validators.required }
            ] as InputValidator[]
        },
        view: {
            label: 'Perfil',
            type: InputType.select,
            options: [
                { value: 'administrador', label: 'Administrador' },
                { value: 'cadastrante', label: 'Cadastrante' },
                { value: 'consultor', label: 'Consultor' },
            ]
        }
    },
    {
        control: {
            name: 'senha',
            value: '',
            isDisabled: false,
            usedAt: [FormType.add, FormType.edit],
            validators: [
                { type: ValidatorType.required, message: 'Campo Obrigatório', validator: Validators.required }
            ] as InputValidator[]
        },
        view: {
            label: 'Senha',
            type: InputType.textspan,
        }
    },
]