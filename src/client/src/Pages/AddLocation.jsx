import React, { useState } from "react";
import PropTypes from "prop-types";
import { useForm, Controller } from "react-hook-form";
import { Button, ButtonGroup, Form, FormGroup, Input, Label } from "reactstrap";
import { AddLocationPicker } from "../Components";
import * as yup from "yup";
import { useAddLocationEvent, isFurtherAwayThen } from "../bin/Hooks";
import { navigate } from "hookrouter";

const AddLocation = ({ userLocation }) => {
  const schema = yup.object().shape({
    name: yup.string().required(),
    location: yup
      .array()
      .required()
      .of(yup.number())
      .min(2)
      .max(2)
      .test(
        "distance",
        "De geselecteerde locaties is verder dan een honderd meter van woa daj nu zit, das de bedoeling niet.",
        value => {
          const tooFar = isFurtherAwayThen(0.1, userLocation, value);
          return !tooFar;
        }
      ),
    price: yup
      .number()
      .required()
      .positive(),
    isDraft: yup.bool().required(),
    remark: yup.string()
  });
  const { register, handleSubmit, errors, setValue, control } = useForm({
    defaultValues: {
      name: "Van Eyck Zwembad",
      location: userLocation,
      price: 2.5,
      isDraft: false,
      remark: ""
    },
    validationSchema: schema,
    reValidateMode: "onChange"
  });

  const saveLocation = useAddLocationEvent();

  const onSubmit = values => {
    saveLocation(values);
    navigate(`/`);
  };

  const [isDraft, setIsDraft] = useState(false);
  const toggleIsDraft = () => {
    setIsDraft(!isDraft);
    setValue("isDraft", !isDraft);
  };

  return (
    <div className={"h-100 bg-white pt-2"}>
      <div className="container">
        <Form onSubmit={handleSubmit(onSubmit)}>
          <h1>E nieuwen toevoegen</h1>
          <FormGroup className={"col-lg-6 px-0"}>
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
          <FormGroup>
            <Label for="location">Locatie*</Label>
            <p className="text-muted">De exacte locatie van waar je nu zit</p>
            <p className="text-muted">
              Mikt zo goed meugelijk, tis de R die telt
            </p>
            <Controller
              name={"location"}
              as={AddLocationPicker}
              control={control}
              userLocation={userLocation}
              onChange={([{ ronny, user }]) => {
                return ronny;
              }}
            />
            {errors.location && (
              <p className={"text-danger"}>{errors.location.message}</p>
            )}
          </FormGroup>
          <FormGroup className={"col-lg-6 px-0"}>
            <Label for={"price"}>Prijs*</Label>
            <Input
              type={"number"}
              name={"price"}
              autoComplete={"off"}
              step="0.01"
              innerRef={register}
              invalid={errors.price}
              placeholder={"Oevele de beeste?"}
            />
          </FormGroup>
          <FormGroup className={"col-lg-6 px-0"}>
            <Label for={"isDraft"}>Ist vant vat?</Label>
            <input type={"hidden"} name={"isDraft"} ref={register} />
            <br />
            <ButtonGroup>
              <Button
                color={"primary"}
                outline={isDraft}
                onClick={toggleIsDraft}
              >
                Nint
              </Button>
              <Button
                color={"primary"}
                outline={!isDraft}
                onClick={toggleIsDraft}
              >
                Joat
              </Button>
            </ButtonGroup>
          </FormGroup>
          <FormGroup className={"col-lg-6 px-0"}>
            <Label for={"remark"}>Opmerking</Label>
            <Input type="textarea" name="remark" innerRef={register} />
          </FormGroup>
          <div className="text-right pb-2">
            <Button type="submit" color="primary">
              Save!
            </Button>
          </div>
        </Form>
      </div>
    </div>
  );
};

AddLocation.propTypes = {
  userLocation: PropTypes.arrayOf(PropTypes.number)
};

export default AddLocation;
