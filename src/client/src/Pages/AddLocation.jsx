import React from "react";
import { useForm } from "react-hook-form";
import { Button, Form, FormGroup, Input, Label } from "reactstrap";

const AddLocation = () => {
  const { register, handleSubmit, errors, watch } = useForm({
    name: "",
    location: [0, 0],
    price: 0,
    isDraft: false,
    remark: ""
  });

  const onSubmit = values => {};

  console.log(watch("name"));

  return (
    <div className={"h-100 bg-white pt-2"}>
      <div className="container">
        <h1>E nieuwen toevoegen</h1>
        <Form onSubmit={handleSubmit(onSubmit)}>
          <FormGroup>
            <Label for="name">Naam*</Label>
            <Input
              type="text"
              name="name"
              autoComplete="off"
              innerRef={register}
              invalid={errors.name}
              placeholder="Officiele name van de plekke woa daj zit"
            />
          </FormGroup>
          <div className="text-right">
            <Button type="submit" color="primary">
              Save!
            </Button>
          </div>
        </Form>
      </div>
    </div>
  );
};

export default AddLocation;
