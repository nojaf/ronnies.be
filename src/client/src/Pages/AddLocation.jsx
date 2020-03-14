import React, { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { Button, ButtonGroup, Form, FormGroup, Input, Label } from "reactstrap";
import { AddLocationPicker } from "../Components";
import * as yup from "yup";
import { useAddLocationEvent, isFurtherAwayThen } from "../bin/Hooks";
import { navigate } from "hookrouter";

const schema = yup.object().shape({
  name: yup.string().required(),
  price: yup
    .number()
    .transform(value => (isNaN(value) ? undefined : value))
    .required("De prijs is verplicht!")
    .positive(),
  isDraft: yup.bool().required(),
  remark: yup.string()
});

const defaultValues = {
  name: "",
  location: [0, 0],
  price: null,
  isDraft: true,
  remark: ""
};

const validateLocationDistance = (ronny, user) => {
  return isFurtherAwayThen(0.1, ronny, user)
    ? {
        values: {},
        errors: {
          location:
            "De geselecteerde locaties is verder dan een honderd meter van woa daj nu zit, das de bedoeling niet."
        }
      }
    : {
        values: {
          location: ronny
        },
        errors: {}
      };
};

const AddLocation = () => {
  const [userLocation, setUserLocation] = useState([0, 0]);
  const [errors, setErrors] = useState({});
  const validationResolver = data => {
    const yupValidation = schema
      .validate(data, { abortEarly: false })
      .then(values => {
        return { values, errors: {} };
      })
      .catch(yupError => {
        const errors = yupError.inner
          ? yupError.inner.reduce((acc, next) => {
              return Object.assign(acc, { [next.path]: next.errors[0] });
            }, {})
          : {};

        return {
          values: {},
          errors
        };
      });

    const locationValidation = Promise.resolve(
      validateLocationDistance(data.location, userLocation)
    );

    return Promise.all([yupValidation, locationValidation]).then(
      ([yupResult, locationResult]) => {
        return {
          values: Object.assign({}, yupResult.values, locationResult.values),
          errors: Object.assign({}, yupResult.errors, locationResult.errors)
        };
      }
    );
  };

  const { register, handleSubmit, setValue } = useForm({
    defaultValues
  });

  const hasError = name => Object.keys(errors).includes(name);

  const saveLocation = useAddLocationEvent();

  const onSubmit = values => {
    validationResolver(values).then(result => {
      if (Object.keys(result.errors).length) {
        setErrors(result.errors);
      } else {
        setErrors({});
        saveLocation(values);
        navigate(`/`);
      }
    });
  };

  const [isDraft, setIsDraft] = useState(defaultValues.isDraft);
  const toggleIsDraft = () => {
    setIsDraft(!isDraft);
    setValue("isDraft", !isDraft);
  };

  const handleLocationChange = ({ ronny, user }) => {
    setValue("location", ronny);
    setUserLocation(user);
  };

  useEffect(() => {
    register({ name: "location" });
    register({ name: "isDraft" });
  }, [register]);

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
              invalid={hasError("name")}
              placeholder="Officiele name van de plekke woa daj zit"
            />
          </FormGroup>
          <FormGroup>
            <Label for="location">Locatie*</Label>
            <p className="text-muted">De exacte locatie van waar je nu zit</p>
            <p className="text-muted">
              Mikt zo goed meugelijk, tis de R die telt
            </p>
            <AddLocationPicker
              onChange={handleLocationChange}
              invalid={hasError("location")}
            />
            {hasError("location") && (
              <p className={"text-danger"}>{errors.location}</p>
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
              invalid={hasError("price")}
              placeholder={"Oevele de beeste?"}
            />
          </FormGroup>
          <FormGroup className={"col-lg-6 px-0"}>
            <Label for={"isDraft"}>Ist vant vat?</Label>
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
          {JSON.stringify(errors)}
        </Form>
      </div>
    </div>
  );
};

AddLocation.propTypes = {};

export default AddLocation;
