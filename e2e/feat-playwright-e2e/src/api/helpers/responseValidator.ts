import { expect } from "@playwright/test";
import Ajv from "ajv";

type AnyResponseLike = {
    ok(): boolean;
    status(): number;
    json?: () => Promise<any>;
};

export class ResponseValidator {
    static expectSuccess(response: { ok(): boolean; status(): number }) {
        expect(
            response.ok(),
            `Expected successful response (2xx), got ${response.status()}`
        ).toBeTruthy();
    }

    //Accepts either-already-parsed JSON (object)  or a Playwright APIResponse-like object that has .json()
     
    static async expectJsonSchema(dataOrResponse: any, schema: object) {
        const data =
            dataOrResponse &&
            typeof dataOrResponse === "object" &&
            typeof dataOrResponse.json === "function"
                ? await dataOrResponse.json()
                : dataOrResponse;

        const ajv = new Ajv({ allErrors: true, strict: false });
        const validate = ajv.compile(schema);
        const valid = validate(data);

        expect(
            valid,
            `Schema validation failed: ${JSON.stringify(validate.errors, null, 2)}`
        ).toBeTruthy();
    }
}
